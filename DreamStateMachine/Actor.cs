using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.Framework;
using DreamStateMachine.Actions;


namespace DreamStateMachine
{
    // Testing
    class Actor:ICloneable
    {
        
        public static event EventHandler<AttackEventArgs> Attack;
        public static event EventHandler<EventArgs> Death;
        public static event EventHandler<AttackEventArgs> Hurt;
        public static event EventHandler<SpawnEventArgs> Spawn;
        

        public ActionList animationList;
        public Dictionary<String, AnimationInfo> animations;
        //public Attributes attributes;
        public Color color;
        public Point curAnimFrame;
        public Texture2D texture;
        public Rectangle attackBox;
        public Rectangle hitBox;
        public Rectangle body;
        public String className;
        public Vector2 acceleration;
        public Vector2 friction;
        public Vector2 movementIntent;
        public Vector2 sightVector;
        public Vector2 velocity;
        public Walk walkAnimation;
        public World world;
        //public Weapon activeWeapon;
        public int maxSpeed;
        public int minSpeed;
        public int health;
        public int reach;
        public int sight;
        public float rotationVelocity;
        public float maxRotationVelocity;
        
        public bool isPlayer;
        public bool isWalking; 
        public bool isAttacking;
        public bool lockedMovement;
        
        public float bodyRotation;
        public float targetRotation;
        
        

        public Actor(Texture2D tex, int width, int height, int texWidth, int texHeight)
        {
            texture = tex;
            animationList = new ActionList(this);
            animations = new Dictionary<String, AnimationInfo>();
            walkAnimation = new Walk(this.animationList, this);
            color = new Color(255, 255, 255, 255);
            attackBox = new Rectangle(0, 0, 0, 0);
            hitBox = new Rectangle(0, 0, width, height);
            body = new Rectangle(0, 0, texWidth, texHeight);
            acceleration = new Vector2(0, 0);
            friction = new Vector2(0, 0);
            movementIntent = new Vector2(0, 0);
            sightVector = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            curAnimFrame = new Point(0, 0);
            maxSpeed = 7;
            minSpeed = 3;
            bodyRotation = 0;
            targetRotation = 0;
            rotationVelocity = 0;
            health = 100;
            maxRotationVelocity = MathHelper.Pi / 16f;
            isAttacking = false;
            isPlayer = false;
            lockedMovement = false;
        }

        public Object Clone()
        {
            Actor actorCopy = new Actor(texture, hitBox.Width, hitBox.Height, body.Width, body.Height);
            actorCopy.animations = animations;
            actorCopy.className = className;
            actorCopy.color = color;
            actorCopy.health = health;
            actorCopy.sight = sight;
            actorCopy.reach = reach;
            return actorCopy;
        }

        public void unlockMovement()
        {
            lockedMovement = false;
        }

        public void lockMovement()
        {
            lockedMovement = true;
        }

        public void onAttack(DamageInfo damageInfo)
        {
            AttackEventArgs attackEventArgs = new AttackEventArgs(damageInfo);
            Attack(this, attackEventArgs);
        }

        public void onKill(DamageInfo damageInfo)
        {
            Death(this, EventArgs.Empty);
        }

        virtual public void onHurt(DamageInfo damageInfo)
        {
            AttackEventArgs attackEventArgs = new AttackEventArgs(damageInfo);
            health -= damageInfo.damage;
            Hurt(this, attackEventArgs);
        }

        virtual public void onSpawn( int spawnType )
        {
            SpawnEventArgs spawnEventArgs = new SpawnEventArgs(spawnType);
            Spawn(this, spawnEventArgs);
        }

        public void setAnimationFrame(int x, int y)
        {
            curAnimFrame.X = x;
            curAnimFrame.Y = y;
        }

        public void setGaze(Point p)
        {
            targetRotation = MathHelper.Pi + (float)Math.Atan2((p.X - this.hitBox.Center.X), -( p.Y - this.hitBox.Center.Y));
        }

        public void setGaze(Vector2 focus)
        {
            targetRotation = MathHelper.Pi + (float)Math.Atan2((focus.X), -(focus.Y));
        }

        public void setPos(int x, int y)
        {
            hitBox.X = x;
            hitBox.Y = y;
            body.X = hitBox.Center.X - (int)(body.Width / 2.0);
            body.Y = hitBox.Center.Y - (int)(body.Height / 2.0);
        }

        public void setPos(Point point)
        {
            hitBox.X = point.X;
            hitBox.Y = point.Y;
            body.X = hitBox.Center.X - (int)(body.Width / 2.0);
            body.Y = hitBox.Center.Y - (int)(body.Height / 2.0);
        }

        public Rectangle getHitBox()
        {
            return hitBox;
        }

        public Rectangle getBodyBox()
        {
            return body;
        }

        public Texture2D getTexture()
        {
            return texture;
        }

        virtual public void update(float dt)
        {
            if ((this.velocity.X != 0 || this.velocity.Y != 0) && this.isWalking && !this.animationList.has(walkAnimation))
            {
                this.animationList.pushFront(walkAnimation);
            }
            
            //if (this.velocity.X == 0 && this.velocity.Y == 0)
            //{
            //    this.isWalking = false;
            //}

            if (this.bodyRotation != this.targetRotation)
            {
                float difference = this.bodyRotation - this.targetRotation;
                if (Math.Abs(difference) > MathHelper.Pi)
                {
                    difference += difference > 0 ? -MathHelper.TwoPi : MathHelper.TwoPi;
                }

                if (difference < 0)
                {
                    if (difference + this.maxRotationVelocity > 0)
                        this.bodyRotation = this.targetRotation;
                    else
                        this.bodyRotation += this.maxRotationVelocity;
                }
                else if (difference > 0)
                {
                    if (difference - this.maxRotationVelocity < 0)
                        this.bodyRotation = this.targetRotation;
                    else
                        this.bodyRotation -= this.maxRotationVelocity;
                }

                this.sightVector.X = (float)Math.Cos(this.bodyRotation + MathHelper.PiOver2);
                this.sightVector.Y = (float)Math.Sin(this.bodyRotation + MathHelper.PiOver2);
            }
            animationList.update(dt);
        }
    }
}
