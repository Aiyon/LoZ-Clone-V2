using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legend_of_Zelda_Clone
{
    class player
    {

        private Vector2 position;
        private int health;
        private int maxHealth;

        public player(Vector2 pos, int hp)
        {
            //health = hearts * 2, because damage = half heart.
            position = pos;
            maxHealth = hp;
            health = maxHealth;
        }

        public void getHeart()
        { health++; }

        public void HPup()
        { maxHealth += 2; }

        public void damage(int amount)
        { health -= amount; }

        public void move(Vector2 offset)
        {
            position += offset;

            position.X = (float)Math.Round(position.X, 3);
            position.Y = (float)Math.Round(position.Y, 3);

        }

        public int getHealth()
        { return health; }

        public Vector2 getPos()
        { return position; }

        public void setPos(Vector2 pos)
        { position = pos; }

    }
}
