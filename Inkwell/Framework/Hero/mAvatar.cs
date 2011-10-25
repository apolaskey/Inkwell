using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Inkwell.Framework;
using Inkwell.Framework.Graphics;
using Inkwell.Framework.Graphics.Data;

namespace Inkwell.Framework
{
    class mAvatar
    {
        public int WalkSpeed = 1;
        private static volatile mAvatar _instance;
        private static object _padlock = new Object();
        Vector3[] LastPositions; // Array of the last 10 positions
        Vector3 Move = Vector3.Zero;
        int iMaxHealth, iCurrentHealth, iMaxInk, iCurrentInk, boundNegX, boundPosX, boundNegY, boundPosY, boundNegZ, boundPosZ, chance, count, Damage;
        Texture2D t2d_AvatarDefend, t2d_AvatarStun;
        public BasicModel PlayerModel, shadowBox;
        BasicModel AttackBox;
        bool FaceingRight, AttackAlive, PlayerStop, PlayerHit;
        public bool Defending;
        public bool PlayerDead, FlipControls, Stun;
        float speed, ArcChange, enemyX, PlayerHitTimer, StunTimer, StunDuration, RegenStart, RegnDelay;

        enum AvatarState // The States of the player
        {
            Standing,
            AttackingLight,
            AttackingHeavy,
            Ability,
            Jumping,
            Falling,
            Talking,
            DashingLeft,
            DashingRight,
            Walking,
            Hit
        }

        enum abilityState // The States of the ability
        {
            None,
            Alice
        }

        AvatarState State = AvatarState.Standing;
        abilityState Ability = abilityState.Alice;
        public mAvatar()
        {
            LastPositions = new Vector3[11];
            iMaxHealth = 100;
            iCurrentHealth = iMaxHealth;
            iMaxInk = 100;
            iCurrentInk = iMaxInk;
            speed = WalkSpeed;
        }
        #region Stats
        /// <summary>Gets the current Health</summary><returns></returns>
        public int GetCurrentHealth
        {
            get
            {
                return iCurrentHealth;
            }
        }
        public int GetMaxHealth
        {
            get { return iMaxHealth; }
        }
        /// <summary>Sets The MaxHealth</summary><param name="Amount"></param>
        public void SetMaxHealth(int Amount)
        {
            iMaxHealth = Amount;
            if (iMaxHealth < 1)
            {
                iMaxHealth = 1;
            }
        }
        /// <summary>Add to the health, use a nagtive number to lower the health DON'T USE THIS IF YOUR ATTACKING THE PLAYER!</summary><param name="Amount"></param>
        public void HealthAdd(int Amount)
        {
            if (Amount < 0 && Defending)
            {
                Amount /= 2;
            }
            iCurrentHealth += Amount;
            if (iCurrentHealth > iMaxHealth)
            {
                iCurrentHealth = iMaxHealth;
            }
            else
            {
                if (iCurrentHealth < 1)
                {
                    PlayerDead = true;
                    iCurrentHealth = 0;
                    mMenu.Peek.WorkerState = mMenu.MenuState.GAMEOVER;
                }
                else
                {
                    PlayerDead = false;
                }
            }
        }
        /// <summary>Gets the current Ink</summary><returns></returns>
        public int GetCurrentInk
        {
            get
            {
                return iCurrentInk;
            }
        }
        /// <summary>Sets The MaxInk</summary><param name="Amount"></param>
        public void SetMaxInk(int Amount)
        {
            iMaxInk = Amount;
            if (iMaxInk < 0)
            {
                iMaxInk = 0;
            }
        }
        /// <summary>Add to the Ink, use a nagtive number to lower the Ink</summary><param name="Amount"></param>
        public void InkAdd(int Amount)
        {
            iCurrentInk += Amount;
            if (iCurrentInk > iMaxInk)
            {
                iCurrentInk = iMaxInk;
            }
            if (iCurrentInk < 0)
            {
                iCurrentInk = 0;
            }
        }
        /// <summary>Bring the player back to full ink and health</summary>
        public void FullRestore()
        {
            State = AvatarState.Standing;
            PlayerHit = false;
            PlayerStop = false;
            PlayerHitTimer = 0;
            FaceingRight = true;
            iCurrentHealth = iMaxHealth;
            iCurrentInk = iMaxInk;

        }
        public bool FacingRight
        {
            get { return FaceingRight;}
        }
        #endregion
        #region Position
        /// <summary>Get up to 10 last positions, pass in a number from 1 to 9 or leave it blank to get the last kown position of the player.</summary><param name="StepsBack"></param><returns></returns>
        public Vector3 GetLastPosition(int StepsBack)//Look at the last 10 positions; the stepsback should be from 1-10
        {
            if (StepsBack > 10)
            {
                StepsBack = 10;
            }
            if (StepsBack < 1)
            {
                StepsBack = 1;
            }
            return LastPositions[StepsBack];
        }
        public Vector3 GetLastPosition()
        {
            return LastPositions[1];
        }
        void UpdateLastPositions() // Updates the lastposition array
        {

            for (int i = 10; i > 0; i--)
            {
                LastPositions[i] = LastPositions[i - 1];
            }
            LastPositions[0] = PlayerModel.Link.Position;
        }
        /// <summary>When moveing the player to a new screen, might want to clear the last kown poition cause they will be off</summary>
        public void ClearLastPositions()
        {
            for (int i = 0; i < 10; i++)
            {
                LastPositions[i] = PlayerModel.Link.Position;
            }
        }
        #endregion
        void Input()// Player's input
        {
            if (mInput.Peek.IsKeyPressed(Keys.F3))
            {
                //if (FlipControls)
                //    FlipControls = false;
                //else
                //    FlipControls = true;
                StunPlayer(600);
            }
            //Input check (not final)
            if (State != AvatarState.Talking && !Stun)
            {
                #region Defend
                if ((((FlipControls) && (mInput.Peek.IsKeyDown(Keys.S))) || ((!FlipControls) && (mInput.Peek.IsKeyDown(Keys.Down)))))
                {
                    if ((mPhysics.Peek.gravity == 0) && (State != AvatarState.Hit) && (State != AvatarState.Jumping) && (State != AvatarState.AttackingLight) && (State != AvatarState.AttackingHeavy) && (State != AvatarState.Falling))
                    {
                        Move = Vector3.Zero;
                        Defending = true;
                        PlayerModel.Link.Texture = t2d_AvatarDefend;
                    }
                }
                else
                {
                    if (Defending)
                    {
                        State = AvatarState.Standing;
                        Defending = false;
                    }
                }
                #endregion
                if (!Defending)
                {
                    if (State != AvatarState.Hit)
                    {
                        #region LightAttack
                        if (((FlipControls) && (mInput.Peek.IsKeyPressed(Keys.A))) || ((!FlipControls) && (mInput.Peek.IsKeyPressed(Keys.Left))))
                        {
                            if ((State != AvatarState.Jumping) && (State != AvatarState.AttackingLight) && (State != AvatarState.AttackingHeavy) && (State != AvatarState.Falling))
                            {
                                mAudio.Peek.PlaySound(mAudio.SoundName.LightAttack);
                                AttackAlive = true;
                                Damage = 8;
                                State = AvatarState.AttackingLight;
                                count = 0;
                                AttackBox.Link.Position = PlayerModel.Link.Position;
                                Move = Vector3.Zero;
                                RegenStart = 0;
                            }
                        }
                        if (State == AvatarState.AttackingLight)
                        {
                            if (count < 20)
                            {
                                if (FaceingRight)
                                {
                                    AttackBox.Link.Position.X += .3f;
                                }
                                else
                                {
                                    AttackBox.Link.Position.X -= .3f;
                                }
                                count++;
                            }
                            else
                            {
                                for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
                                {

                                    mAI.Peek.enemyList[i].Hit = false;

                                }
                                State = AvatarState.Standing;
                                AttackAlive = false;
                            }
                        }
                        #endregion
                        #region HeavyAttack
                        if (((FlipControls) && (mInput.Peek.IsKeyPressed(Keys.D))) || ((!FlipControls) && (mInput.Peek.IsKeyPressed(Keys.Right))))
                        {
                            if ((State != AvatarState.Jumping) && (State != AvatarState.AttackingLight) && (State != AvatarState.AttackingHeavy) && (State != AvatarState.Falling))
                            {
                                mAudio.Peek.PlaySound(mAudio.SoundName.HeavyAttack);
                                AttackAlive = true;
                                Damage = 12;
                                Move = Vector3.Zero;
                                RegenStart = 0;
                                if (FaceingRight)
                                {
                                    AttackBox.Link.Position = new Vector3(PlayerModel.Link.Position.X - 10, PlayerModel.Link.Position.Y, PlayerModel.Link.Position.Z - 10);
                                    ArcChange = 1;
                                }
                                else
                                {
                                    AttackBox.Link.Position = new Vector3(PlayerModel.Link.Position.X + 10, PlayerModel.Link.Position.Y, PlayerModel.Link.Position.Z - 10);
                                    ArcChange = -1;
                                }
                                State = AvatarState.AttackingHeavy;
                                count = 0;
                            }
                        }
                        if (State == AvatarState.AttackingHeavy)
                        {
                            if (count < 40)
                            {
                                if (AttackBox.Link.Position.Z > PlayerModel.Link.Position.Z)
                                {

                                    if (FaceingRight)
                                    {
                                        ArcChange = -.5f;
                                    }
                                    else
                                    {
                                        ArcChange = .5f;
                                    }
                                }

                                AttackBox.Link.Position += new Vector3(ArcChange, 0f, .5f);
                                count++;
                            }
                            else
                            {
                                for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
                                {

                                    mAI.Peek.enemyList[i].Hit = false;

                                }
                                State = AvatarState.Standing;
                                AttackAlive = false;
                            }
                        }
                        #endregion                
                        #region Jump
                        if (mInput.Peek.IsKeyPressed(Keys.Space))
                        {
                            if ((State != AvatarState.Jumping) && (State != AvatarState.AttackingHeavy) && (State != AvatarState.AttackingLight) && State != AvatarState.Ability && mPhysics.Peek.Top)
                            {
                                if (!mPhysics.Peek.jump)
                                {
                                    mAudio.Peek.PlaySound(mAudio.SoundName.Jump);
                                    mPhysics.Peek.SetGravity(2.0f);
                                    mPhysics.Peek.jump = true;
                                    State = AvatarState.Jumping;
                                }
                            }
                        }
                        if (State == AvatarState.Jumping)
                        {
                            if (mPhysics.Peek.gravity > 0)
                            {
                                PlayerModel.Link.Position.Y += mPhysics.Peek.GetGravity();
                            }
                            if (!mPhysics.Peek.jump)
                            {
                                State = AvatarState.Standing;
                                speed = WalkSpeed;
                            }
                        }
                        #endregion
                        #region Ability
                        switch (Ability)
                        {
                            case abilityState.None:
                                break;
                            case abilityState.Alice:
                                if (((FlipControls) && (mInput.Peek.IsKeyPressed(Keys.W))) || ((!FlipControls) && (mInput.Peek.IsKeyPressed(Keys.Up))))
                                {
                                    if ((State != AvatarState.Jumping) && (State != AvatarState.Falling) && (State != AvatarState.Ability) && iCurrentInk > 15)
                                    {
                                        mAudio.Peek.PlaySound(mAudio.SoundName.SpecialOnOff);
                                        InkAdd(-15);
                                        AttackAlive = true;
                                        Move = Vector3.Zero;
                                        State = AvatarState.Ability;
                                        count = 0;
                                        AttackBox.Link.Position = PlayerModel.Link.Position;
                                    }
                                }
                                if (State == AvatarState.Ability)
                                {
                                    if (count < 25)
                                    {
                                        if (FaceingRight)
                                        {
                                            AttackBox.Link.Position.X += .2f;
                                        }
                                        else
                                        {
                                            AttackBox.Link.Position.X -= .2f;
                                        }
                                        count++;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
                                        {

                                            mAI.Peek.enemyList[i].Hit = false;

                                        }
                                        State = AvatarState.Standing;
                                        AttackAlive = false;
                                    }
                                }
                                break;
                        }
                        #endregion
                        Movement();
                        Check();
                        EnemyCollision();
                    }
                    else
                    {
                        PlayerGetsHit();
                    }
                }
            }
            else
            {
                if (mPhysics.Peek.gravity > 0)
                {
                    PlayerModel.Link.Position.Y += mPhysics.Peek.GetGravity();
                }
                if (Stun && State == AvatarState.Hit)
                {
                    PlayerGetsHit();
                }
                Check();
            }
        }
        #region InputStuff
        void Check()
        {
            if (!mPhysics.Peek.Top && mPhysics.Peek.gravity < 0)
            {
                PlayerModel.Link.Position.Y += mPhysics.Peek.GetGravity();
            }
            if (PlayerHit)
            {
                PlayerHitTimer += mTimer.Peek.ElapsedGameTime.Milliseconds;
                if (PlayerHitTimer > 300)
                {
                    PlayerHit = false;
                }
            }
        }
        void Movement()
        {
            if (State != AvatarState.AttackingLight && State != AvatarState.AttackingHeavy && State != AvatarState.Ability)
            {
                if ((!FlipControls && !mInput.Peek.IsKeyPressed(Keys.A) && !mInput.Peek.IsKeyPressed(Keys.S) && !mInput.Peek.IsKeyPressed(Keys.D) && !mInput.Peek.IsKeyPressed(Keys.W)) ||
                    (FlipControls && !mInput.Peek.IsKeyPressed(Keys.Right) && !mInput.Peek.IsKeyPressed(Keys.Left) && !mInput.Peek.IsKeyPressed(Keys.Down) && !mInput.Peek.IsKeyPressed(Keys.Up)))
                {
                    Move = Vector3.Zero;
                    if (State != AvatarState.Ability && State != AvatarState.Jumping)
                        State = AvatarState.Standing;
                }
                if (State != AvatarState.Hit &&(((FlipControls) && (!mInput.Peek.IsKeyDown(Keys.Down)) && (mInput.Peek.IsKeyDown(Keys.Up))) || ((!FlipControls) && (!mInput.Peek.IsKeyDown(Keys.S)) && (mInput.Peek.IsKeyDown(Keys.W)))))//Move Up
                {
                    if (State != AvatarState.Jumping)
                    {
                        State = AvatarState.Walking;
                    }
                    Move.Z--;
                }

                if (State != AvatarState.Hit &&(((FlipControls) && (!mInput.Peek.IsKeyDown(Keys.Up)) && (mInput.Peek.IsKeyDown(Keys.Down))) || ((!FlipControls) && (!mInput.Peek.IsKeyDown(Keys.Up)) && (!mInput.Peek.IsKeyDown(Keys.W)) && (mInput.Peek.IsKeyDown(Keys.S)))))//Move Down
                {
                    if (State != AvatarState.Jumping)
                    {
                        State = AvatarState.Walking;
                    }
                    Move.Z++;
                }
                if (State != AvatarState.Hit &&(((FlipControls) && (!mInput.Peek.IsKeyDown(Keys.Right)) && (mInput.Peek.IsKeyDown(Keys.Left))) || ((!FlipControls) && (!mInput.Peek.IsKeyDown(Keys.D)) && (mInput.Peek.IsKeyDown(Keys.A)))))//Move Left
                {
                    PlayerModel.Link.HorizontalTextureFlip = true;
                    FaceingRight = false;
                    if (State != AvatarState.Jumping)
                    {
                        State = AvatarState.Walking;
                    }
                    Move.X--;
                }
                if (State != AvatarState.Hit &&(((FlipControls) && (!mInput.Peek.IsKeyDown(Keys.Left)) && (mInput.Peek.IsKeyDown(Keys.Right))) || ((!FlipControls) && (!mInput.Peek.IsKeyDown(Keys.A)) && (mInput.Peek.IsKeyDown(Keys.D)))))//Move Right
                {
                    PlayerModel.Link.HorizontalTextureFlip = false;
                    FaceingRight = true;
                    if (State != AvatarState.Jumping)
                    {
                        State = AvatarState.Walking;
                    }
                    Move.X++;
                }
            }
        }
        void EnemyCollision()
        {
            if (AttackAlive)
            {
                for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
                {
                    if (mPhysics.Peek.BoxCollision(AttackBox, mAI.Peek.enemyList[i].enemyModel) > 0 && !mAI.Peek.enemyList[i].Hit)
                    {
                        if (State == AvatarState.Ability && !mAI.Peek.enemyList[i].ImmuneToAbility)
                        {
                            switch (Ability)
                            {
                                case abilityState.None:
                                    break;
                                case abilityState.Alice:
                                    mAI.Peek.enemyList[i].currentState = Enemy.State.KnockBack;
                                    mAI.Peek.enemyList[i].Hit = true;
                                    break;
                            }
                        }
                        else
                        {
                            if (!mAI.Peek.enemyList[i].NPC)
                            {
                                mAI.Peek.enemyList[i].Health -= Damage;
                                mAI.Peek.enemyList[i].Hit = true;
                                chance = Engine.Randomize(0, 10);
                                if (chance > 7)
                                {
                                    //adding chance to damage for more random numbers
                                    mDamage.Peek.critInitialize(Damage + 5, mAI.Peek.enemyList[i].enemyModel.Link.Position);
                                }
                                else
                                {
                                    mDamage.Peek.damageInitialize(Damage, mAI.Peek.enemyList[i].enemyModel.Link.Position);
                                }
                                if (mAI.Peek.enemyList[i].Health < 1 && State != AvatarState.Ability)
                                {
                                    InkAdd(8);
                                }
                                if (State == AvatarState.AttackingLight)
                                {
                                    AttackAlive = false;
                                    break;
                                }
                            }

                        }
                    }
                }
            }
        }
        void PlayerGetsHit()
        {
            if (PlayerStop)
            {
                if (count < 10)
                {
                    PlayerModel.Link.Position.Y += mPhysics.Peek.gravity;

                    if (PlayerModel.Link.Position.X < enemyX)
                    {
                        PlayerModel.Link.Position.X -= 1f;
                    }
                    else
                    {
                        PlayerModel.Link.Position.X += 1f;
                    }
                }
                else
                {
                    if (!mPhysics.Peek.Top)
                    {
                        PlayerModel.Link.Position.Y += mPhysics.Peek.gravity;
                        if (PlayerModel.Link.Position.X < enemyX)
                        {
                            PlayerModel.Link.Position.X -= 1f;
                        }
                        else
                        {
                            PlayerModel.Link.Position.X += 1f;
                        }
                    }
                    else
                    {
                        PlayerStop = false;
                    }
                }
                count++;
            }
            else
            {
                PlayerHit = true;
                PlayerHitTimer = 0;
                State = AvatarState.Standing;
            }
        }
        #endregion
        /// <summary>If player is disabled, use this to return movement to the player.</summary>
        public void Enable()
        {
            
            State = AvatarState.Standing;
        }
        /// <summary>Stop the player's actions</summary>
        public void Disable()
        {
            Move = Vector3.Zero;
            State = AvatarState.Talking;
        }
        #region StatusEffects
        /// <summary>This stuns the player so he can not take any action but still get hit, Duration is in milliseaconds </summary><param name="DurationMilliseconds"></param>
        public void StunPlayer(float Duration)
        {
            Stun = true;
            StunDuration = Duration;
            Move = Vector3.Zero;
            State = AvatarState.Standing;
            AttackAlive = false;
            Defending = false;
            PlayerModel.Link.Texture = t2d_AvatarStun;
        }
        void Stunned()
        { 
            StunTimer += mTimer.Peek.ElapsedGameTime.Milliseconds;
            if (StunDuration < StunTimer)
            {
                StunTimer = 0;
                Stun = false;
            }
        }
        #endregion
        /// <summary>This is to hit the player so that the player will be pushed backed and gave invincibility time
        /// PosX is the X position of what hit the player, its for direction of push back
        /// Damage should be how much damage should be delt to the player.</summary><param name="PosX"></param><param name="damage"></param>
        public void HitPlayer(float PosX, int damage)
        {
            if (State != AvatarState.Hit && !PlayerHit)
            {
                Move = Vector3.Zero;
                mPhysics.Peek.SetGravity(1);
                HealthAdd(-damage);
                enemyX = PosX;
                State = AvatarState.Hit;
                count = 0;
                PlayerStop = true;
                RegenStart = 0;
                for (int i = 0; i < mAI.Peek.enemyList.Count; i++)
                {
                    mAI.Peek.enemyList[i].Hit = false;
                }
                AttackAlive = false;
            }
            if (Defending)
            {
                if (PlayerModel.Link.Position.X < enemyX)
                {
                    PlayerModel.Link.Position.X -= 5f;
                }
                else
                {
                    PlayerModel.Link.Position.X += 5f;
                }
            }
        }
        /// <summary>Updates the player's stuff, put me in the main update</summary><param name="gametime"></param>
        public void Update()
        {
            PlayerModel.Link.OldPosition = PlayerModel.Link.Position;
            if (Stun)
            {
                Stunned();
            }
            if (!Defending && !Stun)
            {
                mAnimation.Peek.initializeHeroAnimation(State);
            }
            mDamage.Peek.Update();
            Input();
            if (Move != Vector3.Zero)
            {
                Move.Normalize();
                PlayerModel.Link.Position += Move * speed;
            } 
            
            RegenStart += mTimer.Peek.ElapsedGameTime.Milliseconds;
            if (RegenStart > 6000)
            {
                RegnDelay += mTimer.Peek.ElapsedGameTime.Milliseconds;
                if (RegnDelay > 180)
                {
                    HealthAdd(1);
                    RegnDelay = 0;
                }
            }
            UpdateLastPositions();
            Bounds();
            shadowBox.Link.Position.Z = mAvatar.Peek.PlayerModel.Link.Position.Z;
            shadowBox.Link.Position.X = mAvatar.Peek.PlayerModel.Link.Position.X - .75f;
            shadowBox.Link.Position.Y -= 5.0f;
        }
        /// <summary>Load the player's content,put me where you load everything. pass it the position of where the player will start</summary><param name="Position"></param>
        public void Load(Vector3 Position)
        {
            mAnimation.Peek.Load();
            mDamage.Peek.Load();
            State = AvatarState.Standing;
            PlayerHit = false;
            PlayerStop = false;
            PlayerHitTimer = 0;
            FaceingRight = true;
            shadowBox = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, "Models\\Basic Objects\\ShadowPlane", Position - new Vector3(0, 50, 0));
            PlayerModel = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, "Models\\Planes\\Plane6", Position);
            PlayerModel.Link.BoundingBox.Min += Engine.TempVector3(5, 0, -1f);
            PlayerModel.Link.BoundingBox.Max += Engine.TempVector3(-5.5f, -3, 1f);
            AttackBox = new BasicModel(Engine.GameContainer, ModelProperties.Alpha, "Models\\Basic Objects\\box", Vector3.Zero);
            AttackBox.Link.Display = false;
            PlayerModel.Link.Position = Position;
            t2d_AvatarDefend = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\Character\\heroDefend0");
            t2d_AvatarStun = Engine.GameContainer.Load<Texture2D>("Textures\\Environment\\Ground\\Grass");
            AttackBox.Link.BoundingBox.Max -= Engine.TempVector3(2, 3, -.5f);
            AttackBox.Link.BoundingBox.Min += Engine.TempVector3(3, 3, -.5f);
            shadowBox.Link.BoundingBox = PlayerModel.Link.BoundingBox;
            shadowBox.Link.Texture = Engine.GameContainer.Load<Texture2D>("Textures\\Animations\\Character\\ShadowPlane");
            SetBounds(-1000, 1000, -1000, 1000, -1000, 1000);
        }
        #region Bounds Andrew Ernst
        public void SetBounds(int bNegX, int bPosX, int bNegY, int bPosY, int bNegZ, int bPosZ)
        {
            boundNegX = bNegX;
            boundPosX = bPosX;
            boundNegY = bNegY;
            boundPosY = bPosY;
            boundNegZ = bNegZ;
            boundPosZ = bPosZ;
        }
        void Bounds()
        {
            if (PlayerModel.Link.Position.X < boundNegX)
            {
                PlayerModel.Link.Position.X = boundNegX;
            }
            if (PlayerModel.Link.Position.X > boundPosX)
            {
                PlayerModel.Link.Position.X = boundPosX;
            }
            if (PlayerModel.Link.Position.Y < boundNegY)
            {
                PlayerModel.Link.Position.Y = boundNegY;
            }
            if (PlayerModel.Link.Position.Y > boundPosY)
            {
                PlayerModel.Link.Position.Y = boundPosY;
            }
            if (PlayerModel.Link.Position.Z < boundNegZ)
            {
                PlayerModel.Link.Position.Z = boundNegZ;
            }
            if (PlayerModel.Link.Position.Z > boundPosZ)
            {
                PlayerModel.Link.Position.Z = boundPosZ;
            }
        }
        #endregion
        public static mAvatar Peek
        {
            get
            {
                /*Check to see if we already initialized our component*/
                if (_instance == null)
                {
                    /*Lock it so another thread cant check it*/
                    lock (_padlock)
                    {
                        /*Check one more time just to be extra careful*/
                        if (_instance == null)
                            _instance = new mAvatar(); //<-- Create our component
                    }
                }
                return _instance;
            }
        }
    }
}
