# SG-006 — Noesis Image Resource Pipeline

**Guideline ID:** SG-006
**Status:** Active
**Version:** 1.1
**Adopted:** 2026-09-11
**Audience:** Designers, technical artists, developers, and AI agents working
on Unity/Noesis UI.
**Related documents:** [UI Architecture and MVVM Boundaries](SG-003-UI-MVVM-ARCHITECTURE.md), [Unity Engineering Standards](../UNITY_ENGINEERING_STANDARDS.md)

## Purpose and scope

Every image shown by a Noesis/XAML view must have one named entry in the
project's shared image resource dictionary. XAML consumes that entry through a
`StaticResource`; it does not locate the image by a relative file path,
assembly-qualified URI, or other direct asset reference.

This keeps image ownership, replacement, import paths, and visual naming in one
place. It also prevents a view from silently depending on the directory it
happens to be stored in.

This guideline applies to raster and vector images used by player-facing UI,
including `Image.Source`, window and button icon attached properties, metric
icons, control templates, backgrounds, overlays, and images inside data
templates. It applies to new UI immediately and to any existing XAML file that
is being changed.

The words **MUST**, **SHOULD**, and **MAY** are normative. A deliberate
exception must be recorded in the relevant task or decision record with an
owner and a removal or review condition.

## 1. Canonical image dictionary

The canonical dictionary is:

```text
LearningIndieDev/Assets/UI/ImageResources.xaml
```

The dictionary owns the mapping from a stable semantic key to the imported
image. The key is part of the UI contract; the source path is an implementation
detail.

Image keys MUST:

- use the `GalapagOS.Image.` namespace;
- describe the visual role or subject, not the directory path;
- remain stable when the underlying art is replaced; and
- distinguish intentional size or crop variants, such as `.32`, `.64`, or
  `.128`.

Example dictionary entries:

```xaml
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <BitmapImage x:Key="GalapagOS.Image.Icon.Sprout"
               UriSource="GalapagOS/Art/Icons/GalapagOS_Icon_Sprout.png" />
  <BitmapImage x:Key="GalapagOS.Image.Species.Rabbit.32"
               UriSource="../Art/Species/Animals/Standardized/32/Animals_01_Rabbit.png" />
</ResourceDictionary>
```

The dictionary MUST be merged into the shared UI resources used by the view.
Standalone resource dictionaries or panels that can be loaded independently
must merge it directly or through a documented parent dictionary. A view must
not rely on an incidental resource merge from an unrelated scene or host.

## 2. XAML consumption rule

Every fixed image reference MUST use `StaticResource`:

```xaml
<Image Source="{StaticResource GalapagOS.Image.Icon.Sprout}"
       Stretch="Uniform" />
```

The same rule applies to Noesis attached properties whose type is
`ImageSource`:

```xaml
<Button game:GalapagOSButton.IconSource="{StaticResource GalapagOS.Image.Icon.Sprout}"
        Content="RESEARCH" />

<HeaderedContentControl
    game:GalapagOSWindow.IconSource="{StaticResource GalapagOS.Image.Icon.Sprout}" />
```

Direct image literals are prohibited in consuming XAML, including:

```xaml
<!-- Prohibited in a view or template. -->
<Image Source="../../GalapagOS/Art/Icons/GalapagOS_Icon_Sprout.png" />
<Image Source="pack://application:,,,/SomeAssembly;component/Art/Icon.png" />
<Button game:GalapagOSButton.IconSource="Art/Icon.png" />
```

`DynamicResource` is not a substitute for this pipeline. Use `StaticResource`
for image values unless a documented runtime theme or content-selection
exception has been approved.

## 3. Add-an-image workflow

When a UI feature needs an image, complete these steps in order:

1. Place the source art in the appropriate `Assets/UI` or `Assets/Art`
   location and preserve the Unity `.meta` file and asset GUID.
2. Configure the Unity import settings appropriate to the art style and its
   Noesis use. For pixel art, retain the approved project filtering, mipmap,
   wrap, and compression settings; do not hide import changes in the XAML.
3. Add exactly one semantic entry to `Assets/UI/ImageResources.xaml`, unless
   the image is an intentional size or crop variant.
4. Ensure the consuming XAML can resolve the dictionary through its resource
   merge.
5. Reference the key with `{StaticResource ...}` from every consumer,
   including attached `IconSource` properties and control templates.
6. Verify the view in Unity/Noesis, including every state or template path that
   displays the image.

Changing the art behind an existing semantic key is preferred to renaming the
key. Rename a key only when its UI meaning changes, and migrate all consumers
in the same change.

## 4. Review and verification gates

A UI image change is complete only when all of the following are true:

- [ ] The image has a semantic entry in `Assets/UI/ImageResources.xaml`.
- [ ] The dictionary is reachable from every consuming XAML resource scope.
- [ ] Every consumer uses `{StaticResource Key}` rather than a path, assembly
      URI, or `DynamicResource`.
- [ ] Attached `ImageSource` properties use the dictionary entry too.
- [ ] No duplicate dictionary keys or path-specific aliases were introduced.
- [ ] Unity/Noesis imports the XAML without missing-resource warnings.
- [ ] The relevant view states render the image at the intended size and
      filtering quality.
- [ ] New or moved assets retain their `.meta` files and GUIDs.
- [ ] A repository search confirms that the change did not add a direct image
      literal to a consuming XAML file.
- [ ] `git diff --check` passes.

Useful review searches include:

```text
rg --pcre2 -n '(?<![A-Za-z])(?:Source|IconSource)="(?!\{)[^"]+\.(png|jpg|jpeg|bmp|gif|svg)' LearningIndieDev/Assets/UI
```

The expected result for production consumers is no matches. Image paths are
expected only inside `ImageResources.xaml` and approved exceptions.

## 5. Existing direct references and exceptions

Existing direct image references are migration debt, not a reason to add more.
New UI must follow this guideline. When an existing XAML file is touched for
feature or polish work, migrate the image references in that file as part of
the same change unless the task explicitly records why that migration is out of
scope.

The following may be exceptions when the limitation is real and documented:

- third-party or package-owned Noesis theme XAML that the project does not
  control;
- a measured custom-rendering or import tool path that does not consume XAML;
- runtime-selected user or research content whose source cannot be known at
  authoring time.

Even in an exception, the UI-facing contract should use an `ImageSource` or
other resolved object rather than passing a file path through ordinary view
markup. Record the owner, reason, affected files, and removal or review
condition next to the exception.

## Revision history

| Version | Date | Change |
| --- | --- | --- |
| 1.0 | 2026-09-11 | Established the Noesis image dictionary and `StaticResource` consumption pipeline. |
| 1.1 | 2026-09-11 | Corrected the enforcement search so dictionary `UriSource` entries are not mistaken for direct consumers. |
