using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Media;

namespace DreamStateMachine
{
    public sealed class SoundManager
    {
        private static volatile SoundManager instance;
        private static object syncRoot = new object();
        static Dictionary<String, Sound> soundPrototypes;
        static Dictionary<String, Song> songPrototypes;
        private Song curSong;
        private SongCollection songCollection;

        private SoundManager()
        {
            MediaPlayer.IsRepeating = false;
            songCollection = new SongCollection();
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

        public void initSoundConfig(ContentManager content, String soundConfigFile, String musicConfigFile)
        {
            soundPrototypes = new Dictionary<String, Sound>();
            songPrototypes = new Dictionary<String, Song>();
            XDocument soundDoc = XDocument.Load(soundConfigFile);
            List<XElement> sounds = soundDoc.Element("Sounds").Elements("Sound").ToList();
            XDocument musicDoc = XDocument.Load(musicConfigFile);
            List<XElement> songs = musicDoc.Element("Songs").Elements("Song").ToList();
            String soundClass;
         
            SoundEffect effect;
            String songName;
            String songPath;
            Song actualSong;
            foreach (XElement sound in sounds)
            {
                soundClass = sound.Attribute("className").Value;
                effect = content.Load<SoundEffect>(sound.Attribute("filePath").Value);
                soundPrototypes[soundClass] = new Sound(effect);
            }
            //foreach (XElement song in songs)
            //{
            //    songName = song.Attribute("name").Value;
            //    songPath = song.Attribute("filePath").Value;
            //    //actualSong = content.Load<Song>("StainedGlassAndSpookySkeletons");
            //    songPrototypes[songName] = content.Load<Song>(songPath);
            //    songCollection.Add(songPrototypes[songName]);
            //}
        }

        public void playSound(String soundClass)
        {
            soundPrototypes[soundClass].playSound();
        }

        public void playSong(String songName)
        {
            //curSong = ;
            //MediaPlayer.Play(songPrototypes[songName]);
            //MediaPlayer.Play(songCollection);
            soundPrototypes[songName].playSound();
        }

        public void switchSong(String firstSong, String secondSong)
        {
            soundPrototypes[firstSong].stopSound();
            soundPrototypes[secondSong].playSound();
            //MediaPlayer.Stop();
            //MediaPlayer.Play(songCollection, 3);
            //MediaPlayer.Stop();
            //MediaPlayer.Play(songPrototypes[secondSong]);
            //MediaPlayer.Queue.ActiveSongIndex = 1;
            //MediaPlayer.Play(
            //MediaPlayer.Pause();
            //MediaPlayer.Stop();
            //MediaPlayer.Queue.ActiveSong.Dispose();
            //MediaPlayer.MoveNext();
        }

        public void update(float dt)
        {
            foreach(KeyValuePair<String, Sound> soundPrototype in soundPrototypes)
            {
                soundPrototype.Value.update(dt);
            }
        }
    }
}
