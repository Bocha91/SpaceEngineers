#if DEBUG
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using VRageMath;
using VRage.Game;
using VRage.Collections;
using Sandbox.ModAPI.Ingame;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.Game.EntityComponents;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;

namespace SpaceEngineers
{
public sealed class Program : MyGridProgram
{
#endif
        //=======================================================================
        //////////////////////////Начало скрипта/////////////////////////////////
        //=======================================================================

        // Стрим автора скрипта https://www.youtube.com/watch?v=3kKIrS0TOFM
        // я тупо скопировал чтобы попробовать

        // Нужно 6 тесiтовых трастеров, с которых снимается тяга для проверки, куда движемся.
        // Называться движки должны со слов "Right", "Left", "Up", "Down", "Forward", "Backward"  
        // Сам гравдрайв - кубик, внутри которого грависферы, накрученные на максимум отталкивания
        // По сторонам блоки искусств. массы с названиями, начиная с "Right", "Left", "Up", "Down", "Forward", "Backward".
        // Аргументы для кнопок в кабине: "Start", "Stop"  

        float ThrustTresh = 250000; // Порог тяги тестового двигателя, начиная с которого подключается гравдрайв.

        bool GravOn = false;
        bool GravSave;
        IMyThrust ThrRight;
        IMyThrust ThrLeft;
        IMyThrust ThrUp;
        IMyThrust ThrDown;
        IMyThrust ThrBackward;
        IMyThrust ThrForward;
        List<IMyThrust> ThrList;

        List<IMyArtificialMassBlock> MassList;
        List<IMyArtificialMassBlock> RightMassList;
        List<IMyArtificialMassBlock> LeftMassList;
        List<IMyArtificialMassBlock> UpMassList;
        List<IMyArtificialMassBlock> DownMassList;
        List<IMyArtificialMassBlock> BackwardMassList;
        List<IMyArtificialMassBlock> ForwardMassList;

        //IMyShipController Cockpit;

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script.
            // Конструктор, вызываемый только один раз каждый сеанс и
            // всегда перед вызовом любого другого метода. Используйте его для
            // инициализируем ваш скрипт.
            // Создаем списки
            ThrList = new List<IMyThrust>();
            MassList = new List<IMyArtificialMassBlock>();
            RightMassList = new List<IMyArtificialMassBlock>();
            LeftMassList = new List<IMyArtificialMassBlock>();
            UpMassList = new List<IMyArtificialMassBlock>();
            DownMassList = new List<IMyArtificialMassBlock>();
            BackwardMassList = new List<IMyArtificialMassBlock>();
            ForwardMassList = new List<IMyArtificialMassBlock>();

            // Находим движки
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(ThrList);
            foreach (IMyThrust thr in ThrList)
            {
                if (thr.CustomName.StartsWith("Right"))
                {
                    ThrRight = thr;
                }
                else if (thr.CustomName.StartsWith("Left"))
                {
                    ThrLeft = thr;
                }
                else if (thr.CustomName.StartsWith("Up"))
                {
                    ThrUp = thr;
                }
                else if (thr.CustomName.StartsWith("Down"))
                {
                    ThrDown = thr;
                }
                else if (thr.CustomName.StartsWith("Backward"))
                {
                    ThrBackward = thr;
                }
                else if (thr.CustomName.StartsWith("Forward"))
                {
                    ThrForward = thr;
                }
            }
            // Находим блоки искусст. массы

            GridTerminalSystem.GetBlocksOfType<IMyArtificialMassBlock>(MassList);
            foreach (IMyArtificialMassBlock mass in MassList)
            {
                if (mass.CustomName.StartsWith("Right"))
                {
                    RightMassList.Add(mass);
                }
                else if (mass.CustomName.StartsWith("Left"))
                {
                    LeftMassList.Add(mass);
                }
                else if (mass.CustomName.StartsWith("Up"))
                {
                    UpMassList.Add(mass);
                }
                else if (mass.CustomName.StartsWith("Down"))
                {
                    DownMassList.Add(mass);
                }
                else if (mass.CustomName.StartsWith("Backward"))
                {
                    BackwardMassList.Add(mass);
                }
                else if (mass.CustomName.StartsWith("Forward"))
                {
                    ForwardMassList.Add(mass);
                }
            }
            GravOn = GravSave;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        } // программ


        public void UpdateGravDrive()
        {
            SetMassGroup(MassList, "Off");

            if (ThrRight.CurrentThrust > ThrustTresh)
            {
                SetMassGroup(RightMassList, "On");
            }
            else if (ThrLeft.CurrentThrust > ThrustTresh)
            {
                SetMassGroup(LeftMassList, "On");
            }

            if (ThrUp.CurrentThrust > ThrustTresh)
            {
                SetMassGroup(UpMassList, "On");
            }
            else if (ThrDown.CurrentThrust > ThrustTresh)
            {
                SetMassGroup(DownMassList, "On");
            }

            if (ThrBackward.CurrentThrust > ThrustTresh)
            {
                SetMassGroup(BackwardMassList, "On");
            }
            else if (ThrForward.CurrentThrust > ThrustTresh)
            {
                SetMassGroup(ForwardMassList, "On");
            }
        }

        public void SetMassGroup(List<IMyArtificialMassBlock> MassGroup, string OnOff)
        {
            foreach (IMyArtificialMassBlock mass in MassGroup)
            {
                mass.ApplyAction("OnOff_" + OnOff);
            }
        }
   
        public void Main(string argument)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked.
            // The method itself is required, but the argument above
            // can be removed if not needed.
            // Основная точка входа скрипта, вызываемая каждый раз
            // вызывается одно из действий Run для программируемого блока.
    
            // Сам метод необходим, но приведенный выше аргумент
            // можно удалить, если не нужно.
            if (argument == "Start") GravOn = true;
            if (argument == "Stop")
            {
                SetMassGroup(MassList, "Off");
                GravOn = false;
            }
            if (GravOn) UpdateGravDrive();
        }


public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means.

            // This method is optional and can be removed if not
            // needed.
            // Вызывается, когда программа должна сохранять свое состояние. использование
            // этот метод для сохранения вашего состояния в поле «Хранение»
            // или некоторые другие средства.

            // Этот метод является необязательным и может быть удален, если не
            // необходимо.
            GravSave = GravOn;
        }

        //=======================================================================
        //////////////////////////Конец скрипта//////////////////////////////////
        //=======================================================================
#if DEBUG
    }
}
#endif


/*
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;
public sealed class Program : MyGridProgram
{
    // НАЧАЛО СКРИПТА
    public Program()
    { }

    public void Main(string args)
    {

    }

    public void Save()
    { }
    // КОНЕЦ СКРИПТА
}
*/
/*
using System.Text;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
using Sandbox.Common;
using Sandbox.Game;
using VRage.Collections;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace ScriptingClass
{
    class MyScript
    {
        IMyGridTerminalSystem GridTerminalSystem = null;
        IMyGridProgramRuntimeInfo Runtime = null;
        Action<string> Echo = null;
        IMyTerminalBlock Me = null;
        //=======================================================================================
        //////////////////////////START//////////////////////////////////////////
        //=======================================================================================

        void Main()
        {

        }

        //=======================================================================================
        //////////////////////////END//////////////////////////////////////////
        //=======================================================================================
    }
}
*/
