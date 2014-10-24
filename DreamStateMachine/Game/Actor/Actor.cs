using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
//using MonoGame.Framework;
using DreamStateMachine.Actions;


namespace DreamStateMachine
{
    class Actor:ICloneable, IDrawable
    {

        public event EventHandler<EventArgs> HitActor;

        public static event EventHandler<EventArgs> LightAttack;
        public static event EventHandler<AttackEventArgs> DamagedPoint;
        public static event EventHandler<EventArgs> Death;
        public static event EventHandler<AttackEventArgs> Hurt;
        public static event EventHandler<PickupEventArgs> OnPickUp;
        public static event EventHandler<EventArgs> Use;
        public static event EventHandler<SpawnEventArgs> Spawn;
        
        public Dictionary<String, AnimationInfo> animations;
        public List<Rectangle> debugSquares;
        public Color color;
        public Point curAnimFrame;
        public Point posOffset;
        public Texture2D texture;
        public Point gripPoint;
        public Rectangle attackBox;
        public Rectangle hitBox;
        public Rectangle body;
        public String className;
        public Vector2 acceleration;
        public Vector2 friction;
        public Vector2 movementIntent;
        public Vector2 sightVector;
        public Vector2 velocity;
        public World world;
        public Weapon activeWeapon;
        public int id;
        public int maxHealth;
        public int maxSpeed;
        public int minSpeed;
        public int health;
        public int reach;
        public int sight;
        public float rotationVelocity;
        public float maxRotationVelocity;
        
        public bool isWalking; 
        public bool lockedMovement;
        
        public float bodyRotation;
        public float targetRotation;
        
        

        public Actor(Texture2D tex, int width, int height, int texWidth, int texHeight)
        {
            texture = tex;
            animations = new Dictionary<String, AnimationInfo>();
            debugSquares = new List<Rectangle>();
            color = new Color(255, 255, 255, 255);
            gripPoint = new Point();
            attackBox = new Rectangle(0, 0, 0, 0);
            body = new Rectangle(0, 0, texWidth, texHeight);
            hitBox = new Rectangle(texWidth / 2 - width / 2, texHeight / 2 - height / 2, width, height);
            acceleration = new Vector2(0, 0);
            friction = new Vector2(0, 0);
            movementIntent = new Vector2(0, 0);
            sightVector = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
            curAnimFrame = new Point(0, 0);
            maxSpeed = 6;
            minSpeed = 3;
            bodyRotation = 0;
            targetRotation = 0;
            rotationVelocity = 0;
            maxHealth = 100;
            health = maxHealth;
            
            maxRotationVelocity = MathHelper.Pi / 16f;
            lockedMovement = false;

            Actor.DamagedPoint += new EventHandler<AttackEventArgs>(Actor_Attacked);

        }

        public void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging=false )
        {
            Rectangle normalizedPosition;
            Rectangle sourceRectangle;
            Texture2D tex;
            tex = this.getTexture();
            normalizedPosition = new Rectangle(this.body.X - drawSpace.X,
                                               this.body.Y - drawSpace.Y,
                                               this.body.Width,
                                               this.body.Height);


            sourceRectangle = new Rectangle(this.curAnimFrame.X * this.body.Width,
                                            this.curAnimFrame.Y * this.body.Height,
                                            this.body.Width,
                                            this.body.Height);

            spriteBatch.Draw(
                tex,
                new Vector2(normalizedPosition.X + this.body.Width / 2, normalizedPosition.Y + this.body.Height / 2),
                sourceRectangle,
                this.color,
                this.bodyRotation,
                new Vector2(normalizedPosition.Width / 2.0f, normalizedPosition.Height / 2.0f),
                1,
                SpriteEffects.None,
                0
            );

            if (activeWeapon != null)
            {
                activeWeapon.draw(spriteBatch, drawSpace, debugTex, debugging);
            }

            if (debugging)
            {
                debugDraw(spriteBatch, drawSpace, debugTex, normalizedPosition, debugSquares);
                debugSquares.Clear();
            }
        }

        public void debugDraw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, Rectangle normalizedPosition, List<Rectangle> debugSquares)
        {
            spriteBatch.Draw(
                    debugTex,
                    new Vector2(this.hitBox.X - drawSpace.X + normalizedPosition.Width / 2.0f, this.hitBox.Y - drawSpace.Y + normalizedPosition.Height / 2.0f),
                    new Rectangle(0, 0, this.hitBox.Width, this.hitBox.Height),
                    new Color(.5f, .5f, .5f, .5f),
                    0,
                    new Vector2(normalizedPosition.Width / 2.0f, normalizedPosition.Height / 2.0f),
                    1,
                    SpriteEffects.None,
                    0
                );

            foreach (Rectangle debugSquare in debugSquares)
            {
                spriteBatch.Draw(
                debugTex,
                new Vector2(debugSquare.X - drawSpace.X + normalizedPosition.Width / 2.0f, debugSquare.Y - drawSpace.Y + normalizedPosition.Height / 2.0f),
                new Rectangle(0, 0, debugSquare.Width, debugSquare.Height),
                new Color(.5f, .5f, .5f, .5f),
                0,
                new Vector2(normalizedPosition.Width / 2.0f, normalizedPosition.Height / 2.0f),
                1,
                SpriteEffects.None,
                0
                );

            }
        }

        public bool isInDrawSpace(Rectangle drawSpace)
        {
            return this.hitBox.Intersects(drawSpace);
        }

        public Object Clone()
        {
            Actor actorCopy = new Actor(texture, hitBox.Width, hitBox.Height, body.Width, body.Height);
            actorCopy.animations = animations;
            actorCopy.className = className;
            actorCopy.color = new Color(color.ToVector3());
            actorCopy.gripPoint = new Point(gripPoint.X, gripPoint.Y);
            actorCopy.maxHealth = maxHealth;
            actorCopy.health = health;
            actorCopy.maxSpeed = maxSpeed;
            actorCopy.sight = sight;
            actorCopy.sightVector = new Vector2(sightVector.X, sightVector.Y);
            actorCopy.reach = reach;
            return actorCopy;
        }

        public void Light_Attack()
        {
            LightAttack(this, EventArgs.Empty);
        }

        public void giveWeapon(Weapon weapon)
        {
            activeWeapon = weapon;
        }

        public void kill()
        {
            this.health = -1;
            Death(this, EventArgs.Empty);
        }

        public void unlockMovement()
        {
            lockedMovement = false;
        }

        public void lockMovement()
        {
            lockedMovement = true;
        }

        public void Actor_Attacked(Object sender, AttackEventArgs attackEventArgs)
        {
            DamageInfo damageInfo = attackEventArgs.damageInfo;
            Actor attacker = (Actor) sender;
            if (this.id != attacker.id)
                handleActorAttack(attackEventArgs.damageInfo);
        }

        public void onAttack(DamageInfo damageInfo)
        {
            AttackEventArgs attackEventArgs = new AttackEventArgs(damageInfo);
            debugSquares.AddRange(attackEventArgs.damageInfo.attackRects);
            DamagedPoint(this, attackEventArgs);
        }

        public void onHitActor()
        {
            HitActor(this, EventArgs.Empty);
        }

        public void onKill(DamageInfo damageInfo)
        {
            Actor.DamagedPoint -= new EventHandler<AttackEventArgs>(Actor_Attacked);
            Death(this, EventArgs.Empty);
        }

        public void onHurt(DamageInfo damageInfo)
        {
            AttackEventArgs attackEventArgs = new AttackEventArgs(damageInfo);
            health -= damageInfo.damage;
            Hurt(this, attackEventArgs);
        }

        public void onPickup(String itemClassName)
        {
            PickupEventArgs pickupEventArgs = new PickupEventArgs(itemClassName);
            OnPickUp(this, pickupEventArgs);
        }

        public void onSpawn( Point spawnTile, int spawnType )
        {
            SpawnEventArgs spawnEventArgs = new SpawnEventArgs(spawnTile, spawnType);
            Spawn(this, spawnEventArgs);
        }

        public void onUse()
        {
            Use(this, EventArgs.Empty);
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
            posOffset.X = x - hitBox.X;
            posOffset.Y = y - hitBox.Y;
            hitBox.X += posOffset.X;
            hitBox.Y += posOffset.Y;
            body.X += posOffset.X;
            body.Y += posOffset.Y;
            if (this.activeWeapon != null)
            {
                this.activeWeapon.calcWeaponPos(this);
            }
        }

        public void setPos(Point point)
        {
            posOffset.X = point.X - hitBox.X;
            posOffset.Y = point.Y - hitBox.Y;
            hitBox.X += posOffset.X;
            hitBox.Y += posOffset.Y;
            body.X += posOffset.X;
            body.Y += posOffset.Y;
            if (this.activeWeapon != null)
            {
                this.activeWeapon.calcWeaponPos(this);
            }
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

        public void handleActorAttack(DamageInfo damageInfo)
        {
            foreach (Rectangle attackRect in damageInfo.attackRects)
            {
                if (this.hitBox.Intersects(attackRect))
                {
                    damageInfo.attacker.onHitActor();
                    this.velocity += damageInfo.attacker.sightVector * 20;
                    this.onHurt(damageInfo);
                    if (this.health <= 0)
                    {
                        this.onKill(damageInfo);
                    }
                    return;
                }
            }
            
        }

        virtual public void update(float dt)
        {

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
                if (this.activeWeapon != null)
                {
                    this.activeWeapon.calcWeaponPos(this);
                }
            }
        }

        public void remove(){
            Actor.DamagedPoint -= new EventHandler<AttackEventArgs>(Actor_Attacked);
        }
    }
}