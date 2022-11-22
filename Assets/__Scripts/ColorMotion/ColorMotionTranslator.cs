using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ColorMotionTranslator : MonoBehaviour
{
    [Inject] private AvatarSpawner avatarSpawner;

    [SerializeField] private Texture2D colorMotionTexture;

    [SerializeField] private Animator animator;

    private HumanPoseHandler humanPoseHandler;
    private HumanPose humanPose;

    // Start is called before the first frame update
    private void Start()
    {
        ResetTexture();
        SetHumanPoseHandler();
    }

    private void ResetTexture()
    {
        var colors = new Color[25];
        Array.Fill(colors, Color.gray);
        colorMotionTexture.SetPixels(0, 0, 5, 5, colors);
        colorMotionTexture.Apply();
    }

    private void SetHumanPoseHandler()
    {
        animator = avatarSpawner.Animator;
        if (avatarSpawner.Avatar != null)
        {
            humanPoseHandler = new HumanPoseHandler(animator.avatar, avatarSpawner.Avatar);

            humanPoseHandler.GetHumanPose(ref humanPose);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (animator == null) return;
        if (colorMotionTexture == null) return;

        if (!animator.gameObject.activeInHierarchy)
        {
            SetHumanPoseHandler();
        }

        if (humanPoseHandler == null) return;

        humanPoseHandler.GetHumanPose(ref humanPose);

        for (var i = 0; i < 55; i++)
        {
            if (humanPose.muscles[i] > 1.0f)
            {
                humanPose.muscles[i] -= Mathf.FloorToInt(humanPose.muscles[i]);
            }
            if (humanPose.muscles[i] < -1.0f)
            {
                humanPose.muscles[i] += Mathf.FloorToInt(Mathf.Abs(humanPose.muscles[i]));
            }
        }

        // Torso and Head
        // Spine: FrontBack, LeftRight, Twist LeftRight
        colorMotionTexture.SetPixel(2, 0, GetColorValue(humanPose.muscles[0], humanPose.muscles[1], humanPose.muscles[2]));
        // Chest: FrontBack, LeftRight, Twist LeftRight
        colorMotionTexture.SetPixel(2, 1, GetColorValue(humanPose.muscles[3], humanPose.muscles[4], humanPose.muscles[5]));
        // Upper Chest: FrontBack, LeftRight, Twist LeftRight
        colorMotionTexture.SetPixel(2, 2, GetColorValue(humanPose.muscles[6], humanPose.muscles[7], humanPose.muscles[8]));
        // Neck: Nod DownUp, Tilt LeftRight, Turn LeftRight
        colorMotionTexture.SetPixel(2, 3, GetColorValue(humanPose.muscles[9], humanPose.muscles[10], humanPose.muscles[11]));
        // Head: Nod DownUp, Tilt LeftRight, Turn LeftRight
        colorMotionTexture.SetPixel(2, 4, GetColorValue(humanPose.muscles[12], humanPose.muscles[13], humanPose.muscles[14]));
        // Left Leg
        // Left Upper Leg: Front-Back, In-Out, Twist In-Out
        colorMotionTexture.SetPixel(3, 1, GetColorValue(humanPose.muscles[21], humanPose.muscles[22], humanPose.muscles[23]));
        // Left Lower Leg: Stretch, Twist In-Out
        colorMotionTexture.SetPixel(4, 1, GetColorValue(humanPose.muscles[24], humanPose.muscles[25], 0));
        // Left Foot: Up-Down, Twist In-Out, Toes Up-Down
        colorMotionTexture.SetPixel(4, 0, GetColorValue(humanPose.muscles[26], humanPose.muscles[27], humanPose.muscles[28]));
        // Right Leg
        // Right Upper Leg: Front-Back, In-Out, Twist In-Out
        colorMotionTexture.SetPixel(2, 1, GetColorValue(humanPose.muscles[29], humanPose.muscles[30], humanPose.muscles[31]));
        // Right Lower Leg: Stretch, Twist In-Out
        colorMotionTexture.SetPixel(0, 1, GetColorValue(humanPose.muscles[32], humanPose.muscles[33], 0));
        // Right Foot: Up-Down, Twist In-Out, Toes Up-Down
        colorMotionTexture.SetPixel(0, 0, GetColorValue(humanPose.muscles[34], humanPose.muscles[35], humanPose.muscles[36]));
        // Left Arm
        // Left Shoulder: Down-Up, Front-Back
        colorMotionTexture.SetPixel(3, 2, GetColorValue(humanPose.muscles[37], humanPose.muscles[38], 0));
        // Left Arm: Down-Up, Front-Back, Twist In-Out
        colorMotionTexture.SetPixel(4, 2, GetColorValue(humanPose.muscles[39], humanPose.muscles[40], humanPose.muscles[41]));
        // Left Forearm: Stretch, Twist In-Out
        colorMotionTexture.SetPixel(4, 3, GetColorValue(humanPose.muscles[42], humanPose.muscles[43], 0));
        // Left Hand: Down-Up, In-Out
        colorMotionTexture.SetPixel(4, 4, GetColorValue(humanPose.muscles[44], humanPose.muscles[45], 0));
        // Right Arm
        // Right Shoulder: Down-Up, Front-Back
        colorMotionTexture.SetPixel(1, 2, GetColorValue(humanPose.muscles[46], humanPose.muscles[47], 0));
        // Right Arm: Down-Up, Front-Back, Twist In-Out
        colorMotionTexture.SetPixel(0, 2, GetColorValue(humanPose.muscles[48], humanPose.muscles[49], humanPose.muscles[50]));
        // Right Forearm: Stretch, Twist In-Out
        colorMotionTexture.SetPixel(0, 3, GetColorValue(humanPose.muscles[51], humanPose.muscles[52], 0));
        // Right Hand: Down-Up, In-Out
        colorMotionTexture.SetPixel(0, 4, GetColorValue(humanPose.muscles[53], humanPose.muscles[54], 0));

        colorMotionTexture.Apply();
    }

    private Color GetColorValue(float red, float green, float blue)
    {
        red = (red + 1f) / 2f;
        green = (green + 1f) / 2f;
        blue = (blue + 1f) / 2f;
        var color = new Color(red, green, blue);
        return color;
    }

    private void OnDestroy()
    {
        ResetTexture();
    }
}
