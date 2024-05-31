# Cinemachine

* Utility components for Cinemachine.
* Unity minimum version: **2022.3**
* Current version: **1.0.0**
* License: **MIT**
* Dependencies:
	- [com.unity.cinemachine: 2.9.7](https://docs.unity3d.com/Packages/com.unity.cinemachine@2.9/changelog/CHANGELOG.html)

## Summary

Utility components for Cinemachine.

## How To Use

### Using ImpulseGenerator

When enabled, this component generates an impulse using a local **CinemachineImpulseSource**.

![ImpulseGenerator](Docs~/ImpulseGenerator.png "Impulse Generator")

Use it on explosions or other Game Objects like that.

> Don't forget to attach a **CinemachineImpulseListener** into your VirtualCamera.

### Using FollowAttacher

At Start, this component will attach a **Transform** to be followed by the local VirtualCamera. The transform will be find using a Tag.

![FollowAttacher](Docs~/FollowAttacher.png "Follow Attacher")

### Using Confiner2DCollider

This component facilitates the creation of **PolygonCollider2D** rectangles to be used by **CinemachineConfiner2D**

![Confiner2DCollider](Docs~/Confiner2DCollider.png "Confiner 2D Collider")

On your VirtualCamera, you may use **Confiner2DAttacher** to attaches a Bounding Shape 2D to a local **CinemachineConfiner2D** on Awake.

![Confiner2DAttacher](Docs~/Confiner2DAttacher.png "Confiner 2D Attacher")

## Installation

### Using the Package Registry Server

Follow the instructions inside [here](https://cutt.ly/ukvj1c8) and the package **ActionCode-Cinemachine** 
will be available for you to install using the **Package Manager** windows.

### Using the Git URL

You will need a **Git client** installed on your computer with the Path variable already set. 

- Use the **Package Manager** "Add package from git URL..." feature and paste this URL: `https://github.com/HyagoOliveira/Cinemachine.git`

- You can also manually modify you `Packages/manifest.json` file and add this line inside `dependencies` attribute: 

```json
"com.actioncode.cinemachine":"https://github.com/HyagoOliveira/Cinemachine.git"
```

---

**Hyago Oliveira**

[GitHub](https://github.com/HyagoOliveira) -
[BitBucket](https://bitbucket.org/HyagoGow/) -
[LinkedIn](https://www.linkedin.com/in/hyago-oliveira/) -
<hyagogow@gmail.com>