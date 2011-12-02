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

namespace XNAlab6
{
   public interface IInputHandler
    {
        KeyboardState KeyboardState { get; }
        MouseState MouseState { get; }
        MouseState OriginalMouseState { get; }
    }; 

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Input : Microsoft.Xna.Framework.GameComponent, IInputHandler
    {
        private KeyboardState keyboardState;
        private MouseState mouseState;
        public MouseState originalMouseState;
        private Boolean first = true;

        public Input(Game game) : base(game)
        {
            game.Services.AddService(typeof(IInputHandler), this);
        }
                
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

           // Exit
           if (keyboardState.IsKeyDown(Keys.Escape))
               Game.Exit();

            base.Update(gameTime);
        }

        public KeyboardState KeyboardState
        {
            get { return (keyboardState); }
        }

        public MouseState MouseState
        {
            get { return (mouseState); }
        }

        public MouseState OriginalMouseState
        {
            get {
                if (first)
                {
                    originalMouseState = mouseState;
                    first = false;
                }
                return (originalMouseState); 
            }
        }
    }
}
