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

    struct MittVerteksFormat
    {
        private Vector3 position;
        private Color color;
        private Vector2 texcoord;
        private Vector3 normal;

        public MittVerteksFormat(Vector3 position, Color color, Vector2 texcoord, Vector3 normal)
        {
            this.position = position;
            this.color = color;
            this.texcoord = texcoord;
            this.normal = normal;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        GraphicsDeviceManager graphics;
        private ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        private Input input;
        private Cam camera;

        VertexPositionColorTexture[] cubeVertices;
        VertexPositionColorTexture[] cubeVertices2;
        VertexPositionColor[] xAxis = new VertexPositionColor[2];
        VertexPositionColor[] yAxis = new VertexPositionColor[2];
        VertexPositionColor[] zAxis = new VertexPositionColor[2];

        Texture2D texture1;
        Texture2D texture2;

        private Effect effect;
        private EffectParameter effectRed;
        private EffectParameter effectPos;
        private EffectParameter effectWorld;
        private EffectParameter effectView;
        private EffectParameter effectProjection;
        private float mfRed = 0f;
        private bool mbRedIncrease = true;

        private Matrix world;
        private Matrix projection;
        private Matrix view;

        private Vector3 cameraPosition = new Vector3(5f, 4f, -4f);
        private Vector3 cameraTarget = Vector3.Zero;
        private Vector3 cameraUpVector = new Vector3(0.0f, 1.0f, 0.0f);

        private const float BOUNDARY = 80.0f;
        private const float EDGE = BOUNDARY * 2.0f;

        float orbRotY;

        private float speed = 0.02f;
        float elapsedTime;

        TimeSpan fpsTime = TimeSpan.Zero;
        int frameRate = 0;
        int frameCounter = 0;
        private Video video;
        private VideoPlayer player;
        private VertexPositionColorTexture[] frontV;
        private VertexPositionColorTexture[] leftV;
        private VertexPositionColorTexture[] backV;
        private VertexPositionColorTexture[] rightV;
        private VertexPositionColorTexture[] topV;
        private Texture2D texture3;
        private Texture2D texture4;


        public Game1()
        {
            this.IsFixedTimeStep = false;
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(this.Services);

            //Oppretter og tar i bruk input-handleren: 
            input = new Input(this);
            this.Components.Add(input);

            //Legger til Cam:
            camera = new Cam(this);
            this.Components.Add(camera);
        }

        protected override void Initialize()
        {
            base.Initialize();
            InitDevice();
            InitCamera();
            InitVertices();
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
        /// Position the camera.
        /// </summary>
        private void InitCamera()
        {
            //Projeksjon:
            float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;

            //Oppretter view-matrisa:
            Matrix.CreateLookAt(ref cameraPosition, ref cameraTarget, ref cameraUpVector, out view);

            //Oppretter projeksjonsmatrisa:
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.01f, 1000.0f, out projection);

        }

        /// <summary>
        /// Prepare the object vertices
        /// </summary>
        protected void InitVertices()
        {
            // Set axis lines
            xAxis[0] = new VertexPositionColor(new Vector3(-100.0f, 0f, 0f), Color.Red);
            xAxis[1] = new VertexPositionColor(new Vector3(100.0f, 0f, 0f), Color.Red);
            yAxis[0] = new VertexPositionColor(new Vector3(0f, -100.0f, 0f), Color.White);
            yAxis[1] = new VertexPositionColor(new Vector3(0f, 100.0f, 0f), Color.White);
            zAxis[0] = new VertexPositionColor(new Vector3(0f, 0f, -100.0f), Color.Black);
            zAxis[1] = new VertexPositionColor(new Vector3(0f, 0f, 100.0f), Color.Black);

            // min and max for texture
            float min = 0.001f; float max = 0.999f;

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
                new VertexPositionColorTexture(new Vector3(-1, 1, -1),
                    Color.Green, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(1, 1, -1),
                    Color.Green, new Vector2(min,min))
            };

            // Initialize a Cube
            
            cubeVertices = new VertexPositionColorTexture[8]
            {
                new VertexPositionColorTexture(new Vector3(1, -1,  1),
                    Color.Blue, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3( 1,  1,  1),
                    Color.Blue, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(-1, -1,  1),
                    Color.Red, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(-1, 1,  1),
                    Color.Red, new Vector2(max,min)),
                new VertexPositionColorTexture(new Vector3(-1, -1, -1),
                    Color.Green, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(-1, 1, -1),
                    Color.Green, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(1, -1, -1),
                    Color.Yellow, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(1, 1, -1),
                    Color.Yellow, new Vector2(max,min))
            };
            cubeVertices2 = new VertexPositionColorTexture[8]
            {
                new VertexPositionColorTexture(new Vector3(1, -1, -1),
                    Color.Green, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(1, -1,  1),
                    Color.Red, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(1,  1, -1),
                    Color.Yellow, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3( 1,  1,  1),
                    Color.Blue, new Vector2(max,min)),
                new VertexPositionColorTexture(new Vector3(-1,  1, -1),
                    Color.Yellow, new Vector2(min,max)),
                new VertexPositionColorTexture(new Vector3(-1,  1,  1),
                    Color.Blue, new Vector2(min,min)),
                new VertexPositionColorTexture(new Vector3(-1, -1, -1),
                    Color.Green, new Vector2(max,max)),
                new VertexPositionColorTexture(new Vector3(-1, -1,  1),
                    Color.Red, new Vector2(max,min))
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

                effectRed = effect.Parameters["fx_Red"];
                //effectPos = effect.Parameters["fx_Pos"];

                // Load textures
                texture1 = Content.Load<Texture2D>(@"Content/potato");
                texture2 = Content.Load<Texture2D>(@"Content/turtle");
                texture3 = Content.Load<Texture2D>(@"Content/uwp");
                texture4 = Content.Load<Texture2D>(@"Content/underwater");

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

            SetPos(gameTime);

            elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            fpsCalc(gameTime);

            base.Update(gameTime);
        }

        // FPS calculation
        void fpsCalc(GameTime gameTime)
        {
            fpsTime += gameTime.ElapsedGameTime;

            if (fpsTime > TimeSpan.FromSeconds(1))
            {
                fpsTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        void SetPos(GameTime gameTime)
        {
            if (mbRedIncrease)
                mfRed += (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            else
                mfRed -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            if (mfRed <= 0.0f)
                mbRedIncrease = true;
            else if (mfRed >= 1.0f)
                mbRedIncrease = false;

            effectRed.SetValue(mfRed);
        }

        private void DrawAxis()
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, xAxis, 0, 1);
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, yAxis, 0, 1);
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, zAxis, 0, 1);
            }
        }

        private void DrawOverlayText(string text, int x, int y)
        {
            spriteBatch.Begin();
            //Skriver teksten to ganger, først med gul bakgrunn og deretter med rød, en piksel ned og til venstre, slik at teksten får en skygge.
            spriteBatch.DrawString(spriteFont, text, new Vector2(x, y), Color.Yellow);
            spriteBatch.DrawString(spriteFont, text, new Vector2(x - 1, y - 1), Color.Red);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }

        private void DrawSide(VertexPositionColorTexture[] vertices, Texture2D texture)
        {
            Matrix matIdentify = Matrix.Identity;
            Matrix scale, cubeTrans, orbRotatY;

            Matrix.CreateScale(0.5f, 0.5f, 0.5f, out scale);

            cubeTrans = Matrix.CreateTranslation(2f, 0f, -2f);

            orbRotatY = Matrix.CreateRotationY(orbRotY);
            orbRotY += (elapsedTime * speed) / 150f;
            orbRotY = orbRotY % (float)(2 * Math.PI);

            world = matIdentify *orbRotatY;

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

            //DrawAxis();
            //DrawCube(cubeVertices, texture1);
            //DrawCube(cubeVertices2, texture2);

            DrawSkyBox();

            // Teller frames og viser FPS
            frameCounter++;
            DrawOverlayText(string.Format("FPS: {0}", frameRate), 5, 2);

            base.Draw(gameTime);
        }

        private void DrawSkyBox()
        {
            // If the video is playing, get the current frame
            // as a texture. (Calling GetTexture on a stopped
            // player results in an exception)
            if (player.State == MediaState.Playing)
                texture3 = player.GetTexture();

            DrawSide(frontV, texture3);
            DrawSide(leftV, texture3);
            DrawSide(backV, texture1);
            DrawSide(rightV, texture2);
        }
        
    }
}
