using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;

namespace DreamStateMachine.Behaviors
{
    class ItemManager
    {

        //Dictionary<String, Actor> itemPrototypes;
        Dictionary<String, Weapon> weaponPrototypes;

        public ItemManager()
        {
            //itemPrototypes = new Dictionary<string, Actor>();
            weaponPrototypes = new Dictionary<string, Weapon>();

            Actor.OnPickUp += new EventHandler<PickupEventArgs>(Actor_Pickup_Item);
        }

        public void initWeaponConfig(ContentManager content, String weaponConfigFile)
        {
            XDocument weaponDoc = XDocument.Load(weaponConfigFile);
            List<XElement> weapons = weaponDoc.Element("Weapons").Elements("Weapon").ToList();
            List<XElement> animations;
            List<XElement> stances;

            Dictionary<String, String> weaponAnimations;
            Dictionary<String, Stance> weaponStances;
            String weaponName;
            Texture2D weaponTex;
            String animationName;
            String animationType;
            int weaponDamage;
            String stanceName;
            Vector2 gripPoint;
            Rectangle drawBox;

            foreach (XElement weapon in weapons)
            {
                weaponAnimations = new Dictionary<string,string>();
                weaponStances = new Dictionary<string,Stance>();
                
                weaponName = weapon.Attribute("name").Value;
                weaponTex = content.Load<Texture2D>(weapon.Attribute("texture").Value);
                weaponDamage = int.Parse(weapon.Attribute("damage").Value);
                animations = weapon.Elements("Animation").ToList();

                foreach(XElement animation in animations){
                    animationName = animation.Attribute("name").Value;
                    animationType = animation.Attribute("type").Value;
                    weaponAnimations[animationType] = animationName;
                }

                stances = weapon.Elements("Stance").ToList();

                foreach(XElement stance in stances){
                    stanceName = stance.Attribute("name").Value;
                    gripPoint = new Vector2();
                    gripPoint.X = float.Parse(stance.Attribute("gripX").Value);
                    gripPoint.Y = float.Parse(stance.Attribute("gripY").Value);
                    drawBox = new Rectangle();
                    drawBox.X = int.Parse(stance.Attribute("texX").Value);
                    drawBox.Y = int.Parse(stance.Attribute("texY").Value);
                    drawBox.Width = int.Parse(stance.Attribute("texWidth").Value);
                    drawBox.Height = int.Parse(stance.Attribute("texHeight").Value);
                    weaponStances[stanceName] = new Stance
                    {
                        name = stanceName,
                        gripPoint = gripPoint,
                        drawBox = drawBox
                    };
                }

                weaponPrototypes[weaponName] = new Weapon(weaponName, weaponTex, weaponDamage, weaponAnimations, weaponStances);

            }
        }

        private void Actor_Pickup_Item(Object sender, PickupEventArgs pickupEventArgs)
        {
            Actor pickingUpActor = (Actor)sender;
            pickingUpActor.giveWeapon((Weapon)weaponPrototypes[pickupEventArgs.itemClassName].Clone());
        }

    }
}
