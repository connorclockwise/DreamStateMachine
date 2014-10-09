using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;

namespace DreamStateMachine
{
    class Sound
    {
        public SoundEffect effect;

        public Sound(SoundEffect effect)
        {
          this.effect = effect;
        }

        public void playSound()
        {
            effect.Play();
        }

     }
}
