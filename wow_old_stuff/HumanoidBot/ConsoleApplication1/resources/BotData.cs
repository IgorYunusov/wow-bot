using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningBot.resources {
    class BotData {
//----------------------1. hely------------------------------------------------------------------------------------------------------
        private static Position3D startingPlace11 = new Position3D(286, -3868, 137);
        private static Position3D startingPlace12 = new Position3D(321, -3223, 132);
        public static Position3D[] startingPlaces1 = { startingPlace11/*, startingPlace12 */};

        private static Position3D[] pointsToGo11 = {   
                                                new Position3D(275, -3887, 137), new Position3D(286, -3896, 137), new Position3D(309, -3891, 137), 
                                                new Position3D(308, -3912, 137), new Position3D(359, -3889, 127), new Position3D(338, -3858, 119), 
                                                new Position3D(351, -3848, 120), new Position3D(362, -3858, 119), new Position3D(398, -3871, 120), 
                                                new Position3D(376, -3887, 120), new Position3D(397, -3918, 120), new Position3D(397, -3885, 120), 
                                                new Position3D(429, -3891, 104), new Position3D(428, -3905, 104), new Position3D(431, -3917, 104), 
                                                new Position3D(447, -3917, 104), new Position3D(461, -3917, 104), new Position3D(450, -3904, 104), 
                                                new Position3D(461, -3903, 104), new Position3D(467, -3894, 104), new Position3D(450, -3891, 105), 
                                                new Position3D(469, -3880, 104), new Position3D(459, -3864, 104), new Position3D(444, -3871, 104), 
                                                new Position3D(426, -3836, 104), new Position3D(431, -3813, 104), new Position3D(430, -3796, 104), 
                                                new Position3D(421, -3750, 104), new Position3D(417, -3723, 104), new Position3D(396, -3725, 104), 
                                                new Position3D(393, -3743, 103), new Position3D(358, -3727, 104), new Position3D(356, -3748, 104), 
                                                new Position3D(316, -3754, 104), new Position3D(283, -3753, 104), new Position3D(268, -3754, 104), 
                                                new Position3D(269, -3745, 104), new Position3D(310, -3758, 106), new Position3D(290, -3808, 116), 
                                                new Position3D(248, -3795, 120), new Position3D(267, -3835, 120),  
                                              /*new Position3D(274, -3893, 138), new Position3D(269, -3916, 139), new Position3D(282, -3884, 137), 
                                              new Position3D(286, -3899, 137), new Position3D(316, -3888, 135), new Position3D(302, -3914, 137), 
                                              new Position3D(359, -3890, 127), new Position3D(335, -3852, 120), new Position3D(357, -3844, 120), 
                                              new Position3D(371, -3871, 121), new Position3D(378, -3899, 120), new Position3D(384, -3911, 120), 
                                              new Position3D(398, -3919, 120), 
                                              new Position3D(391, -3922, 120), new Position3D(398, -3887, 120), new Position3D(431, -3893, 104), 
                                              new Position3D(429, -3907, 104), new Position3D(428, -3918, 104), new Position3D(450, -3915, 104), 
                                              new Position3D(463, -3917, 104), new Position3D(463, -3903, 104), new Position3D(448, -3904, 104), 
                                              new Position3D(448, -3904, 104), new Position3D(448, -3890, 105), new Position3D(469, -3897, 104), 
                                              new Position3D(470, -3877, 104), new Position3D(457, -3861, 104), new Position3D(443, -3875, 104), 
                                              new Position3D(426, -3835, 104), new Position3D(429, -3812, 104), new Position3D(434, -3764, 104)*/ };

        private static Position3D[] pointsToGo12 = {    
                                               new Position3D(300, -3220, 137), new Position3D(306, -3242, 137), new Position3D(341, -3240, 127), 
                                               new Position3D(338, -3215, 127), new Position3D(354, -3228, 127), new Position3D(361, -3275, 119),
                                               new Position3D(363, -3272, 119), 
                                               new Position3D(340, -3304, 120), new Position3D(348, -3322, 120), new Position3D(359, -3357, 120), 
                                               new Position3D(382, -3357, 120), new Position3D(357, -3325, 120), new Position3D(393, -3330, 120), 
                                               new Position3D(371, -3293, 120), new Position3D(401, -3299, 120), new Position3D(396, -3270, 120), 
                                               new Position3D(391, -3241, 120), 
                                               new Position3D(402, -3226, 120), new Position3D(363, -3233, 119), new Position3D(380, -3193, 120), 
                                               new Position3D(404, -3196, 120), new Position3D(374, -3219, 120)};
        public static Position3D[][] farmingPlaces1 = { pointsToGo11/*, pointsToGo12 */};

        public static String[] elitesArr1 = { "Shadowsworn Drakonid", "Netharel" };
//----------------------2. hely------------------------------------------------------------------------------------------------------
        private static Position3D startingPlace21 = new Position3D(262, -3686, 90);
        public static Position3D[] startingPlaces2 = { startingPlace21};

        private static Position3D[] pointsToGo21 = {
                                                    new Position3D(248, -3691, 90), new Position3D(248, -3678, 90), new Position3D(251, -3686, 91), 
                                                    new Position3D(273, -3689, 90), new Position3D(323, -3687, 90), new Position3D(352, -3695, 91), 
                                                    new Position3D(356, -3684, 90), new Position3D(407, -3689, 90), new Position3D(438, -3691, 90), 
                                                    new Position3D(464, -3687, 90), new Position3D(483, -3698, 90), new Position3D(500, -3685, 90), 
                                                    new Position3D(494, -3695, 90), new Position3D(511, -3730, 94), new Position3D(507, -3761, 90), 
                                                    new Position3D(495, -3776, 90), new Position3D(513, -3780, 86), new Position3D(527, -3781, 77), 
                                                    new Position3D(497, -3805, 89), new Position3D(494, -3829, 90), new Position3D(512, -3826, 86),
                                                    new Position3D(515, -3845, 91), new Position3D(508, -3863, 91), 
                                                    new Position3D(509, -3863, 91), new Position3D(513, -3848, 91), new Position3D(509, -3819, 85), 

                                                    new Position3D(531, -3824, 79), new Position3D(550, -3827, 72), new Position3D(569, -3826, 72), 
                                                    new Position3D(575, -3818, 72), 
                                                    new Position3D(515, -3845, 91), new Position3D(508, -3863, 91), 
                                                    new Position3D(509, -3863, 91), new Position3D(513, -3848, 91), new Position3D(509, -3819, 85), 
                                                    new Position3D(509, -3819, 85), new Position3D(566, -3838, 72), new Position3D(566, -3861, 73), 
                                                    new Position3D(547, -3858, 76), new Position3D(561, -3838, 72), new Position3D(565, -3824, 72), 
                                                    new Position3D(574, -3808, 71), new Position3D(558, -3793, 70), 
                                                    new Position3D(574, -3772, 65), new Position3D(575, -3744, 55), new Position3D(574, -3727, 49), 
                                                    new Position3D(575, -3704, 40), new Position3D(573, -3680, 32), new Position3D(572, -3663, 26), 
                                                    new Position3D(572, -3645, 20), new Position3D(562, -3645, 20), new Position3D(562, -3626, 15), 
                                                    new Position3D(570, -3631, 15), new Position3D(561, -3624, 15), new Position3D(561, -3646, 20), 
                                                    new Position3D(551, -3646, 20), new Position3D(550, -3661, 26), new Position3D(552, -3683, 33), 
                                                    new Position3D(560, -3704, 40), new Position3D(552, -3724, 48), new Position3D(551, -3750, 59), 
                                                    new Position3D(550, -3769, 65)
                                                   };
        public static Position3D[][] farmingPlaces2 = { pointsToGo21};
        
        public static String[] elitesArr2 = { "Sunfury Eradicator" };
    }
}
