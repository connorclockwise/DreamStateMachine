using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;


namespace DreamStateMachine.Input
{
    class MoveCommand: Command
    {
        Vector2 movement;
        public MoveCommand(Vector2 movement)
        {
            this.movement = movement;
        }
        public override void Execute(Actor player)
        {
            if (!player.lockedMovement && movement.Length() > .1f)
            {
                movement.Normalize();
                player.movementIntent = movement * 0.7f;
                player.isWalking = true;
            }
            else
            {
                player.movementIntent.X = 0;
                player.movementIntent.Y = 0;
                player.isWalking = false;
            }
        }
        public override void Execute(UIComponent uiComponent)
        {
            
            if (uiComponent.children.Count > 0)
            {
                int curChildIndex = 0;
                UIComponent curChild = uiComponent.children[curChildIndex];
                while (!curChild.hasFocus && curChildIndex < uiComponent.children.Count - 1)
                {
                    curChildIndex++;
                    curChild = uiComponent.children[curChildIndex];
                }

                if (curChild.hasFocus)
                {
                    movement.Normalize();
                    if (movement.Y > 0)
                    {
                        if (curChildIndex == 0)
                        {
                            curChildIndex = uiComponent.children.Count - 1;
                        }
                        else
                            curChildIndex--;
                    }
                    else if (movement.Y < 0)
                    {
                        if (curChildIndex == uiComponent.children.Count - 1)
                        {
                            curChildIndex = 0;
                        }
                        else
                            curChildIndex++;
                    }
                    uiComponent.children[curChildIndex].giveFocus();
                }
                else
                {
                    uiComponent.children[0].giveFocus();
                }
                
            }
        }
    }
}
