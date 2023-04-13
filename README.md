<div align="center">

![readme splash](https://imgur.com/YRCq0WE.png)

<br>

## [**Installation**](#installation)
## [**Features**](#features)
## [**WIP**](#wip)
## [**License**](./LICENSE.md)

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

* 

# WIP

These tasks are the current planned features that are on the roadmap to be either completed *(in progress)* or started *(backlog)*

- [ ] 