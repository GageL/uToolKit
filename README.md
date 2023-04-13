<div align="center">

![readme splash](https://imgur.com/obFceQg.png)

A collection of tools made for the Unity Engine that cover systems and features such as Logging, UI, Editor and Runtime Logic. This packages purpose is to assist with everyday development and to aid in cutting down the need to write repetative systems for projects.

<br>

Note: All elements of this package are subject to change anytime so be sure to backup your projects before updating to new versions.

<br>

### [**Installation**](#installation)
### [**Features**](#features)
### [**WIP**](#wip)
### [**License**](./LICENSE.md)

<br>
<br>

> Source: This distributed unity asset to be ingested through the Unity Package Manager. It contains custom systems, managers, extensions and utilities both for Runtime and Editor use.

> Licensing: Any external non-affiliated packages or assets included in this project and repository have either CC0 or MIT release.

</div>

# Installation

### Dependencies

*For any UPM external libraries below, simply navigate to your project `manifest.json` file located within your project's Packages directory and add the following lines.*

#### API Compatability Level

If using the `Player Prefs Manager` the `API Compatability Level` in the `Project Settings` must be set to `.NET 4.X` and does not support `.NET Standard 2.0`

#### DOTween (HOTween v2)

> ANIMATE ANYTHING via simplified code. DOTween is a fast, efficient, fully type-safe object-oriented animation engine.

**[Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)**

#### Odin Inspector and Serializer

> Odin puts your Unity workflow on steroids, making it easy to build powerful and advanced user-friendly editors for you and your entire team.

**[Asset Store](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)**

#### TextMeshPro

> For this package, be sure to import the default package included with TMP (Window =-> TextMeshPro -> Import TMP Essential Resources)

> Additionally, the current implemented version `3.0.6` includes specific settings for the UI components. Any previous version may contain bugs, graphic artifacts or redactions in functionality

```json
"com.unity.textmeshpro": "3.0.6"
```

### Usage

Add the **[uToolKit](https://github.com/GageL/uToolKit)** package to your project by editing the `manifest.json` file as such:

```json
"com.gagel.utoolkit": "https://github.com/GageL/uToolKit.git"
```

# Features

### Editor

#### Calculator

A simple calculator to run simple math equations within. Formats similar to the Windows calculator. Includes the following features:

![calculator](https://imgur.com/kcKYcgI.png)

* Operations to log output
* Debug menu (all operations)
* Add - Subtract - Multiply - Divide - Invert Value

---

#### Find Script Utility

Provides the ability to search the project and scene for a specific class assigned to a `GameObject`, either as a scene object or prefab. Includes the following features:

![find-script-utility](https://imgur.com/1iulHpV.png)

* Find null components
* Manual script name via string
* Script via drap-drop file
* Scene search
* Project search
* Search results list
* List item selection/highlight

---

#### Input Commands

Includes editor keybinds for miscellaneous actions. Includes the following features:

* Clear editor console output

---

#### Player Prefs Manager

Pulls from the registry where Unity `PlayerPrefs` are stored from the editor and showcases the key/value data in a table format. Includes the following features:

![player-prefs-manager](https://imgur.com/iqC4ZqX.png)

* Add new pref
    * Key
        * String
    * Value
        * Int
        * Float
        * String
* Edit inline pref toggle
* Delete inline pref
* Prompt for adding new pref
* Prompt for deleting inline pref
* Scrollable list

---

#### Script Template Formatter

This is a script that works as part of the `AssetModificationProcessor`. Essentially it looks for a custom substring within the script template text file and supplies said area with key inputs, namely:

* Company Name ***(relies on `Company Name` being set within `Player Settings`)***
* Product Name ***(relies on `Product Name` being set within `Player Settings`)***
* Creation Date

> Script Template File - `{DRIVE}:\{PATH_TO_EDITOR}\{EDITOR_VERSION}\Editor\Data\Resources\ScriptTemplates\81-C# Script-NewBehaviourScript.cs.txt`

Simply replace that text file with the text file from this ***[DOWNLOAD](https://drive.google.com/file/d/1GqCdX_4IgLGnpkRScWHKmM1HE5zFICDi/view?usp=sharing)*** link.

**Here is a sample of a processed new class:**

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompanyName.ProductName {
	public class TestClass : MonoBehaviour {
		#region Public/Private Variables

		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods

		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
}
```

### Runtime

#### AudioSourceInterpolater.cs

Contains Tweening logic for easing into a track/clip to a target volume over a specified amount of time.

#### InitSiblingIndex.cs

Contains logic that sets the sibling index of the object the component is attached to. Includes 3 modes to reference at runtime such as `Editor` and `Runtime` via `Awake`, `Start` and `Update`. Note, running sibling index via `Update` can affect overall performance.

#### LoggerOptions.cs

Contains a custom console log that automatically links to Unitys `Debug.Log` callback. Additionally, it has the ability to intake custom commands that are broadcast via an `Action` for any external class to subscribe to to then parse and run logic against. Includes a themed color pallete and font.

#### UIButton.cs

Extends the functionality of the base Unity `Button` UI class and adds more flexibility and features to control more than 1 element within a buttons visibility. Includes the following features:

* Modes
    * None: Useful for buttons that have no graphic change needed
    * Color
    * Sprite: Requires premade sprites set for each input state
* Targets *(based upon the `Graphic` base class)*
    * Image
    * Text: Default support for `Unity Text` or `TextMeshPro`
* Input State
    * Idle
    * Highlighted
    * Pressed
    * Selected
    * Selected Highlighted
    * Selected Pressed
    * Disabled
* Toggle Selection Capability
* Animator System Based *( `Mecanim` )*
* Events *( `UnityEvent` )*
    * Pointer Enter/Exit/Down/Up/Click

#### UIFadeControl.cs

Allows any RectTransform -> GameObject the ability to have fading capabilities driven through Tween logic. The meat of the interactability is based upon its dependency on the *`CanvasGroup`* component, primarily the *`Alpha`*, *`Interactable`* and *`Blocks Raycasts`* members. Includes the following features:

* Start Shown
* Fade In/Out Duration
* Fade In/Out Ease
* Events *( `UnityEvent` )*
    * Show Started/Completed
    * Hide Started/Completed
* Show/Hide/Instant Control methods

#### Utils.cs

Includes extension methods for common data types such as string, float, int and vectors as well as helper utility methods.

# WIP

These tasks are the current planned features that are on the roadmap to be either completed *(in progress)* or started *(backlog)*

- [ ] Simple Event Dispatcher System