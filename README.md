# RubeGoldbergMachineActivity

A Unity physics activity with 12 colorful interactions built from primitive spheres and cubes. The machine demonstrates ramps, dominoes, hinged seesaws, pendulums, a gate, a springy bumper, a collapsing tower, and a finish trigger.

## Run the activity

1. Open or create a Unity project and copy `Assets/Scripts/RubeGoldbergMachine.cs` into its `Assets/Scripts` folder.
2. Create an empty GameObject in a scene and add the `RubeGoldbergMachine` component.
3. Enter Play mode and press **Space** to launch the marble. Press **R** to reset the machine.

The script builds the runway, camera, lighting, materials, physics objects, and interaction progress display when the component starts. The 12 stations use Rigidbody physics, colliders, and hinge joints; checkpoint triggers keep the marble moving through the full demonstration.

Built for the Video Game Development course at Universidad del Valle, under Professor Gisler Garcés.

## Acknowledgments

These 3D models were used in this project, all from Sketchfab:

- **"Dio the Dinosaur (Scary Monsters)"** by [annzep](https://sketchfab.com/annzep)
- **"3D Model Menacing ゴ Symbol - Jojo"** by [adamw1806](https://sketchfab.com/adamw1806)
