﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DreamStateMachine.Actions;
using Microsoft.Xna.Framework;

namespace DreamStateMachine.Input
{
    class RotateWeaponCommand: Command
    {
        public override void Execute(Actor player)
        {
            //player.onPickup("bone");
            //List<Rectangle> attackRect = new List<Rectangle>();
            //attackRect.Add(player.hitBox);
            //player.onHurt(new DamageInfo(player, 100, attackRect));
            player.rotateWeapon();
        }

        public override void Execute(Game.GUI.UIComponent uiComponent)
        {
            throw new NotImplementedException();
        }
    }
}
