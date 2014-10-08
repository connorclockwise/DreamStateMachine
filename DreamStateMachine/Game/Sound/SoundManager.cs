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
    public sealed class SoundManager
    {
        private static volatile SoundManager instance;
        private static object syncRoot = new object();
        static Dictionary<String, Sound> soundPrototypes;

        private SoundManager()
        {
           
        }

        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SoundManager();
                    }
                }
                return instance;
            }
        }

        public void initSoundConfig(ContentManager content, String soundConfigFile)
        {
            soundPrototypes = new Dictionary<String, Sound>();
            var doc = XDocument.Load(soundConfigFile);
            var sounds = doc.Element("Sounds").Elements("Sound");
            String soundClass;
            SoundEffect effect;
            foreach (XElement sound in sounds)
            {
                soundClass = sound.Attribute("className").Value;
                effect = content.Load<SoundEffect>(sound.Attribute("filePath").Value);
                soundPrototypes[soundClass] = new Sound(effect);
            }
        }

        public void playSound(String soundClass)
        {
            soundPrototypes[soundClass].playSound();
        }
    }
}
