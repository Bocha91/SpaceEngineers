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
        IMyTextPanel DBG;
        //IMyReflectorLight Light;
        IMyCockpit cocpit;
        bool memcocpit = false;
        public Program()
        {
            DBG = GridTerminalSystem.GetBlockWithName("DBG") as IMyTextPanel;
            DBG.ShowPublicTextOnScreen();
            DBG.WritePublicText("Отладочная консоль\n", false);

            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(blocks);

            initLight(blocks); // настройка всех ламп и прожекторов

            // создать группу
            //IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName( "Группа");

            
            // печать имён всех групп корабля
            List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
            GridTerminalSystem.GetBlockGroups(groups);
            foreach (IMyBlockGroup tut in groups)
            {
                DBG.WritePublicText(tut.Name.ToString() + "\n", true);
            }

            Echo("Вывод в терминал программного блока");

            // разпознавание присутствия хозяина в кокпите
            cocpit = GridTerminalSystem.GetBlockWithName("Кабина") as IMyCockpit;
            

        } // программ
        public void Main(string argument)
        {
            if (argument == "stop") return;
            else Runtime.UpdateFrequency = UpdateFrequency.Update100;
            
            // разпознавание присутствия хозяина в кокпите
            if (memcocpit != cocpit.IsUnderControl)
            {
                memcocpit = cocpit.IsUnderControl;
                if (memcocpit) DBG.WritePublicText("Вошол\n", true);
                else DBG.WritePublicText("Вышел\n", true);
            }

        }
        public void Save()
        {
        }
        //=======================================================================
        //////////////////////////Конец скрипта//////////////////////////////////
        //=======================================================================

        
        public void initLight(List<IMyTerminalBlock> blocks)
        {
            DBG.WritePublicText("#"+blocks.Count.ToString() + "\n", true);
            foreach (IMyLightingBlock light in blocks)
            {
                DBG.WritePublicText(light.ToString() + "\n", true);
                light.SetValueColor("Color", new Color(255, 255, 255));
                light.Radius = 160;
                light.Intensity = 1.2f;
                light.ShowInTerminal = false;
                light.BlinkIntervalSeconds = 0;
                light.BlinkLength = 10.0f;
                light.BlinkOffset = 0;
                light.Enabled = false;
            }
        }

#if DEBUG
    }
}
#endif

