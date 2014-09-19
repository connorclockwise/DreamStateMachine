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
    class SoundManager
    {
        Dictionary<int, Sound> soundPrototypes;

        public SoundManager()
        {
            soundPrototypes = new Dictionary<int, Sound>();
        }

        public void initSoundConfig(ContentManager content, String soundConfigFile)
        {
            var doc = XDocument.Load(soundConfigFile);
            var sounds = doc.Element("Sounds").Elements("Sound");
            String soundClass;
            SoundEffect effect;
            int soundID;
            foreach (XElement sound in sounds)
            {
                soundClass = sound.Attribute("className").Value;
                effect = content.Load<SoundEffect>(sound.Attribute("filePath").Value);
                soundID = int.Parse(sound.Attribute("soundID").Value);
                this.soundPrototypes[soundID] = new Sound(effect);
                this.soundPrototypes[soundID].className = soundClass;
                this.soundPrototypes[soundID].soundID = soundID;
            }
        }

        public void playSound(int soundID)
        {
            soundPrototypes[soundID].playSound();
        }

        public void update(float dt)
        {

        }
    }
}
