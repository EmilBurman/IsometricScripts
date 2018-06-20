using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CloudsOfAvarice
{
    public static class Inputs
    {
        //Movement
        public const string HORIZONTALMOVEMENT = "Horizontal";
        public const string VERTICALMOVEMENT = "Vertical";
        public const string JUMP = "Jump";
        public const string SPRINT = "Sprint";
        public const string INTERACT = "Interact";
        public const string ACTION = "Action";

        //Combat
        public const string ATTACK = "Attack";
        public const string PARRY = "Parry";
        public const string THROW = "Throw";
        public const string AIM = "Aim";
        public const string SWITCHWEAPON = "WeaponSwitch";
        public const string DRAWWEAPON = "WeaponDraw";
        public const string HEAL = "Heal";
        public const string TARGET = "Target";
        public const string RETARGET = "ReTarget";

        //Dashing
        public const string DASH = "Dash";
        public const string HORIZONTALDASH = "HorizontalDash";
        public const string VERTICALDASH = "VerticalDash";

        //Camera
        public const string SWITCHCAMERARIGHT = "CameraRight";
        public const string SWITCHCAMERALEFT = "CameraLeft";
    }
}