using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Legend_of_Zelda_Clone
{
    public static class Globals
    {
        private enum OWTexVals : int    //values for overworld textures. More for confirming map values are correct than for actual use.
        {
            stair = 0,
            rock,
            empty,
            TEBrownTL,    //TE = Tree Entrance (dungeons)
            TEBrownT,
            TEBrownTR,
            greenStair,
            greenRock,
            empty2,
            TEGreenTL,
            TEGreenT,
            TEGreenTR,
            whiteStair,
            whiteRock,
            emptyGrey,
            TEwhiteTL,
            TEwhiteT,
            TEwhiteTR,
            blueStairBrown,
            treeBrown,
            statueBrown,
            treeBrownBL,
            treeBrownB,
            treeBrownBR,
            blueStairGreen,
            treeGreen,
            statueGreen,
            treeGreenBL,
            treeGreenB,
            treeGreenBR,
            blueStairWhite,
            graveWhite,
            statueWhite,
            treeWhiteBL,
            treeWhiteB,
            treeWhiteBR,
            brownWallTL,
            brownWallT,
            brownWallTR,
            brownDETL, //DE = dungeon entrace
            brownDET,
            brownDETR,
            greenWallTL,
            greenWallT,
            greenWallTR,
            greenDETL,
            greenDET,
            greenDETR,
            whiteWallTL,
            whiteWallT,
            whiteWallTR,
            whiteDETL,
            whiteDET,
            whiteDETR,

        }

        public static int UIOffset = 56;

        public enum gState : int
        {
            overworld = 0,
            dungeon1,
            dungeon2,
            caves,
            menus,

        }

        public enum NPCs : int
        {
            man = 0,
            woman,
            boy,
            goblin,
        }
    }
}
