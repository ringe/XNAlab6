using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Runtime.InteropServices;

namespace XNAlab6
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        GraphicsDeviceManager graphics;
        private ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        private Input input;
        private Camera camera;

        Texture2D texture1;
        Texture2D texture2;

        private Effect effect;
        private EffectParameter effectWorld;
        private EffectParameter effectView;
        private EffectParameter effectProjection;

        private Matrix world;

        private const float BOUNDARY = 80.0f;
        private const float EDGE = BOUNDARY * 2.0f;

        float rotY;

        private float speed = 0.02f;
        float elapsedTime;

        private Video video;
        private VideoPlayer player;
        private VertexPositionColorTexture[] frontV;
        private VertexPositionColorTexture[] leftV;
        private VertexPositionColorTexture[] backV;
        private VertexPositionColorTexture[] rightV;
        private VertexPositionColorTexture[] topV;
        private Texture2D texture3;
        private Texture2D texture4;
        private Texture2D surfaceTexture;
        private Texture2D bottomTexture;
        private VertexPositionColorTexture[] bottomV;


        public Game1()
        {
            this.IsFixedTimeStep = false;
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(this.Services);

            //Oppretter og tar i bruk input-handleren: 
            input = new Input(this);
            this.Components.Add(input);

            //Legger til Cam:
            camera = new FirstPersonCamera(this);
            this.Components.Add(camera);
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitDevice();
            InitVertices();
            camera.Initialize();
        }

        /// <summary>
        /// Initialize the graphics device.
        /// </summary>
        private void InitDevice()
        {

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;


            Window.Title = "Rotating Cube";
        }

        /// <summary>
        /// Prepare the object vertices
        /// </summary>
        protected void InitVertices()
        {

            // min and max for texture
            float min = 0.001f; float max = 0.999f;

            bottomV = new VertexPositionColorTexture[4] {
                new VertexPositionColorTexture(new Vector3(1, -1, 1),
                    Color.Green, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(-1, -1, 1),
                    Color.Green, new Vector2(max,min)),
                new VertexPositionColorTexture(new Vector3(1, -1, -1),
                    Color.Green, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(-1, -1, -1),
                    Color.Green, new Vector2(min,max))
            };

            // Front plane
            frontV = new VertexPositionColorTexture[4] {
                
                new VertexPositionColorTexture(new Vector3(-1, -1, 1),
                    Color.Yellow, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(-1, 1, 1),
                    Color.Yellow, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(1, -1, 1),
                    Color.Yellow, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(1, 1, 1),
                    Color.Yellow, new Vector2(max,min))
            };

            leftV = new VertexPositionColorTexture[4] {
                new VertexPositionColorTexture(new Vector3(-1, -1, -1),
                    Color.Green, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(-1, 1, -1),
                    Color.Green, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(-1, -1,  1),
                    Color.Green, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(-1, 1,  1),
                    Color.Green, new Vector2(max,min))
            };

            backV = new VertexPositionColorTexture[4] {
                
                new VertexPositionColorTexture(new Vector3(1, -1, -1),
                    Color.Yellow, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(1, 1, -1),
                    Color.Yellow, new Vector2(max,min)),
                new VertexPositionColorTexture(new Vector3(-1, -1, -1),
                    Color.Yellow, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(-1, 1, -1),
                    Color.Yellow, new Vector2(min,min))
            };

            rightV = new VertexPositionColorTexture[4] {
                
                new VertexPositionColorTexture(new Vector3(1, -1, -1),
                    Color.Green, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(1, 1, -1),
                    Color.Green, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(1, -1,  1),
                    Color.Green, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(1, 1,  1),
                    Color.Green, new Vector2(max,min))
            };

            topV = new VertexPositionColorTexture[4] {
                new VertexPositionColorTexture(new Vector3(1, 1, 1),
                    Color.Green, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(-1, 1, 1),
                    Color.Green, new Vector2(max,min)),
                new VertexPositionColorTexture(new Vector3(1, 1, -1),
                    Color.Green, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(-1, 1, -1),
                    Color.Green, new Vector2(min,max))
            };
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>(@"Content/Arial");

            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);

            //Initialize Effect
            try
            {
                effect = Content.Load<Effect>(@"Content/MinEffekt");
                effectWorld = effect.Parameters["World"];
                effectView = effect.Parameters["View"];
                effectProjection = effect.Parameters["Projection"];

                // Load textures
                texture1 = Content.Load<Texture2D>(@"Content/potato");
                texture2 = Content.Load<Texture2D>(@"Content/turtle");
                texture3 = Content.Load<Texture2D>(@"Content/uwp");
                texture4 = Content.Load<Texture2D>(@"Content/underwater");
                surfaceTexture = Content.Load<Texture2D>(@"Content/5980020-ocean-surface");
                bottomTexture = Content.Load<Texture2D>(@"Content/hazelnut_lg");

                // Load a video, and initialize a player
                video = Content.Load<Video>(@"Content/uw");
                player = new VideoPlayer();
                player.IsLooped = true;

            }
            catch (ContentLoadException cle)
            {
                MessageBox(new IntPtr(0), cle.Message, "Utilgivelig feil...", 0);
                this.Exit();
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Play the video if it isn't already.
            if (player.State != MediaState.Playing)
                player.Play(video);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            base.Update(gameTime);
        }

        private void DrawSide(VertexPositionColorTexture[] vertices, Texture2D texture)
        {
            Matrix matIdentify = Matrix.Identity;
            Matrix rotatY;

            rotatY = Matrix.CreateRotationY(rotY);
            rotY += (elapsedTime * speed) / 150f;
            rotY = rotY % (float)(2 * Math.PI);

            world = matIdentify * rotatY;

            GraphicsDevice.Textures[0] = texture;

            effectWorld.SetValue(world);
            effectView.SetValue(camera.View);
            effectProjection.SetValue(camera.Projection);

            BlendState bs = new BlendState();
            bs.AlphaSourceBlend = Blend.One;
            bs.AlphaDestinationBlend = Blend.BlendFactor;
            bs.ColorSourceBlend = Blend.One;
            bs.ColorDestinationBlend = Blend.SourceColor;
            bs.AlphaBlendFunction = BlendFunction.Subtract;
            GraphicsDevice.BlendState = bs;
            
            //Starter tegning av cuben
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2, VertexPositionColorTexture.VertexDeclaration);
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            rasterizerState1.FillMode = FillMode.Solid;
            GraphicsDevice.RasterizerState = rasterizerState1;

            GraphicsDevice.Clear(Color.DeepSkyBlue);

            //Setter world=I:
            world = Matrix.Identity;

            effectWorld.SetValue(world);
            effectView.SetValue(camera.View);
            effectProjection.SetValue(camera.Projection);

            DrawSkyBox();

            base.Draw(gameTime);
        }

        private void DrawSkyBox()
        {
            // If the video is playing, get the current frame
            // as a texture. (Calling GetTexture on a stopped
            // player results in an exception)
            if (player.State == MediaState.Playing)
                texture3 = player.GetTexture();

            DrawSide(bottomV, bottomTexture);
            DrawSide(backV, texture1);
            DrawSide(rightV, texture2);
            DrawSide(frontV, texture3);
            DrawSide(leftV, texture3);
            DrawSide(topV, surfaceTexture);
        }
        
    }
}
