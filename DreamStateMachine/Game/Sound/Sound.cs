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
        float curFadeTime;
        float endFadeTime;
        bool isFadingIn = false;
        bool isFadingOut = false;

        public Sound(SoundEffect effect)
        {
            curFadeTime = 0;
            this.effect = effect.CreateInstance();
        }

        public void playSound()
        {
            effect.Play();
        }

        public void playLoopedSound()
        {
            effect.IsLooped = true;
            effect.Play();
        }

        public void stopSound()
        {
            effect.Stop();
        }

        public void fadeInSound(float fadeTime)
        {
            this.playLoopedSound();
            curFadeTime = 0;
            endFadeTime = fadeTime;
            isFadingIn = true;
            effect.Volume = 0;
        }

        public void fadeOutSound(float fadeTime)
        {
            curFadeTime = 0;
            endFadeTime = fadeTime;
            isFadingOut = true;
            effect.Volume = 1;
        }

        public void update(float dt)
        {
            if (isFadingIn)
            {
                curFadeTime += dt;
                if (curFadeTime >= endFadeTime)
                {
                    effect.Volume = 1.0f;
                    isFadingIn = false;
                    curFadeTime = 0;
                }
                else
                {
                    effect.Volume = (curFadeTime / endFadeTime); 
                }
            }
            else if (isFadingOut)
            {
                curFadeTime += dt;
                if (curFadeTime >= endFadeTime)
                {
                    effect.Volume = 0.0f;
                    isFadingOut = false;
                    curFadeTime = 0;
                }
                else
                {
                    effect.Volume = ((endFadeTime - curFadeTime) / endFadeTime);
                }
            }
        }

     }
}
