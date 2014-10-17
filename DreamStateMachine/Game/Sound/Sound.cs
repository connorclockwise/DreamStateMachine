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
        public SoundEffectInstance effect;
        float startFade;
        bool isFadingIn = false;
        bool isFadingOut = false;

        public Sound(SoundEffect effect)
        {
          this.effect = effect.CreateInstance();
        }

        public void playSound()
        {
            effect.Play();
        }

        public void fadeInSound(float timeToFadeIn)
        {
            effect.Volume = 0;
        }

     }
}
