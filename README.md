# Unity Tube Renderer
3D version of unity LineRenderer

TubeRenderer is a little bit of a misnomer since it depends on the MeshRenderer, and MeshFilter, built-in components to do the actual rendering.

## Purpose
* Move this fairly simple utility out of larger more complicated libraries which do this as a secondary or tertiary goal.
* To allow for scripted objects to display 3 dimensional paths and lines (such as a simulated rope, dynamic cables, or pipes)
* To allow detail level to be configured with scripting

### Using Git

Make sure the Git client is installed on your marchine and that you have added the Git executable path to your `PATH` environment variable.

Navigate to `%ProjectFolder%/Packages/` and open the `manifest.json` file.

in the "dependencies" section add:

```json
{
  "dependencies": {
      ...
      "com.github.oparaskos.unity.tube.renderer": "git+https://github.com/oparaskos/unity-tube-renderer.git#0.1.0",
      ...
  }
}
```

Find more information about this [here](https://docs.unity3d.com/Manual/upm-git.html).


## TODO: 
* Allow End caps (capping with circles + allow prefabs to be placed at start and end)
* Start/End width to Thickness as a curve
* Allow editing via controlpoints in editor
* Automated LOD.
* Smoothing for interpolated points