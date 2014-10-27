using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine2.game.World
{
    class EnemyConfig
    {
        public String enemyClass;
        public int difficulty;

        public EnemyConfig(String enemyClass, int enemyDifficulty)
        {
            this.enemyClass = enemyClass;
            this.difficulty = enemyDifficulty;
        }
    }
}
