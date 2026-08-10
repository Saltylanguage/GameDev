//#define DEBUG_IMPORTER

using UnityEditor;
using UnityEngine;
using UnityEditor.AssetImporters;
using UnityEngine.Video;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

[ScriptedImporter(2, "xaml")]
class NoesisXamlImporter : ScriptedImporter
{
    [UnityEditor.InitializeOnLoadMethod]
    static void RegisterApplicationResourcesHash()
    {
        string projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
        string filename = System.IO.Path.Combine(projectPath, "Library", "Noesis", "ApplicationResources_Hash");

        if (File.Exists(filename))
        {
            try
            {
                string hash = File.ReadAllText(filename);
                AssetDatabase.RegisterCustomDependency("Noesis_ApplicationResources", Hash128.Parse(hash));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        } 
    }

    [Serializable]
    struct Dependency
    {
        public enum Type
        {
            File,
            Font,
            UserControl,
            Shader
        }

        public Type type;
        public string uri;
    }

    [Serializable]
    struct Dependencies
    {
        public string hash;
        public long timestamp;
        public List<Dependency> items;
    }

    static string HashFile(string path)
    {
        var hash = new Hash128();
        hash.Append(File.ReadAllText(path));
        return hash.ToString();
    }

    static string HashStr(string str)
    {
        var hash = new Hash128();
        hash.Append(str);
        return hash.ToString();
    }

    static string DependenciesPath()
    {
        string projectPath = System.IO.Path.GetDirectoryName(Application.dataPath);
        return System.IO.Path.Combine(projectPath, "Library", "Noesis", "Dependencies");
    }

    static bool LoadDependencies(string path, ref Dependencies deps)
    {
        string folder = DependenciesPath();
        string filename = Path.Combine(folder, HashStr(path));

        if (File.Exists(filename))
        {
            try
            {
                string json = File.ReadAllText(filename);
                deps = JsonUtility.FromJson<Dependencies>(json);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        return false;
    }

    static void SaveDependencies(string path, Dependencies deps)
    {
        try
        {
            string folder = DependenciesPath();
            System.IO.Directory.CreateDirectory(folder);

            string json = JsonUtility.ToJson(deps);
            File.WriteAllText(Path.Combine(folder, HashStr(path)), json);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    static IEnumerable<string> FindFonts(string uri)
    {
        int index = uri.IndexOf('#');
        if (index != -1)
        {
            string folder = uri.Substring(0, index);
            if (Directory.Exists(folder))
            {
                string family = uri.Substring(index + 1);
                var files = Directory.EnumerateFiles(folder).Where(s => IsFont(s));

                foreach (var font in files)
                {
                    using (FileStream file = File.Open(font, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        if (NoesisUnity.HasFamily(file, family))
                        {
                            yield return font;
                        }
                    }
                }
            }
        }
    }

    static Dependencies GetCachedDependencies(string path)
    {
        Dependencies deps = new Dependencies();
        bool cached = LoadDependencies(path, ref deps);

        long timestamp = 0;

        try
        {
            timestamp = File.GetLastWriteTime(path).Ticks;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        if (cached)
        {
            if (timestamp == deps.timestamp)
            {
                return deps;
            }

            deps.timestamp = timestamp;

            string hash = HashFile(path);
            if (hash == deps.hash)
            {
                SaveDependencies(path, deps);
                return deps;
            }

            deps.hash = hash;
        }
        else
        {
            deps.timestamp = timestamp;
            deps.hash = HashFile(path);
        }

      #if DEBUG_IMPORTER
        Debug.Log($"=> Dependencies {path}");
      #endif

        deps.items = new List<Dependency>();

        string[] xamlList = null;
        string[] effectList = null;
        string[] brushList = null;

        using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            Noesis.GUI.GetXamlDependencies(file, path, (uri_, type) =>
            {
                try
                {
                    string uri = Noesis.UriHelper.GetPath(uri_);

                    if (type == Noesis.XamlDependencyType.Filename)
                    {
                        deps.items.Add(new Dependency{ type = Dependency.Type.File, uri = uri});
                    }
                    else if (type == Noesis.XamlDependencyType.Font)
                    {
                        foreach (var font in FindFonts(uri))
                        {
                            deps.items.Add(new Dependency{ type = Dependency.Type.Font, uri = font});
                        }
                    }
                    else if (type == Noesis.XamlDependencyType.UserControl)
                    {
                        if (uri.EndsWith("Effect"))
                        {
                            if (effectList == null)
                            {
                                effectList = Directory.EnumerateFiles(Application.dataPath, "*.noesiseffect", SearchOption.AllDirectories)
                                    .Select(path => path.Replace(Application.dataPath, "Assets"))
                                    .Select(path => path.Replace("\\", "/"))
                                    .ToArray();
                            }

                            foreach (var effect in effectList)
                            {
                                if (Path.GetFileNameWithoutExtension(effect) == uri.Replace("Effect", ""))
                                {
                                    deps.items.Add(new Dependency{ type = Dependency.Type.Shader, uri = effect});
                                    break;
                                }
                            }
                        }
                        else if (uri.EndsWith("Brush"))
                        {
                            if (brushList == null)
                            {
                                brushList = Directory.EnumerateFiles(Application.dataPath, "*.noesisbrush", SearchOption.AllDirectories)
                                    .Select(path => path.Replace(Application.dataPath, "Assets"))
                                    .Select(path => path.Replace("\\", "/"))
                                    .ToArray();
                            }

                            foreach (var brush in brushList)
                            {
                                if (Path.GetFileNameWithoutExtension(brush) == uri.Replace("Brush", ""))
                                {
                                    deps.items.Add(new Dependency{ type = Dependency.Type.Shader, uri = brush});
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (xamlList == null)
                            {
                                xamlList = Directory.EnumerateFiles(Application.dataPath, "*.xaml", SearchOption.AllDirectories)
                                    .Select(path => path.Replace(Application.dataPath, "Assets"))
                                    .Select(path => path.Replace("\\", "/"))
                                    .ToArray();
                            }

                            foreach (var xaml in xamlList)
                            {
                                if (Path.GetFileNameWithoutExtension(xaml) == uri)
                                {
                                    if (xaml != path)
                                    {
                                        deps.items.Add(new Dependency{ type = Dependency.Type.UserControl, uri = xaml});
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            });
        }

        SaveDependencies(path, deps);
        return deps;
    }

    static string[] GatherDependenciesFromSourceFile(string path)
    {
        NoesisUnity.InitCore();
        List<string> deps = new List<string>();

        try
        {
            foreach (var dep in GetCachedDependencies(path).items)
            {
                if (File.Exists(dep.uri))
                {
                    deps.Add(dep.uri);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return deps.ToArray();
    }

    static void AddFont(string uri, ref List<NoesisFont> fonts)
    {
        if (!String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(uri)))
        {
            NoesisFont font = AssetDatabase.LoadAssetAtPath<NoesisFont>(uri);

            if (font != null)
            {
                fonts.Add(font);
            }
        }
    }

    static void AddTexture(string uri, ref List<Texture> textures, ref List<Sprite> sprites)
    {
        if (!String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(uri)))
        {
            if (AssetImporter.GetAtPath(uri) is TextureImporter textureImporter)
            {
                if (!AssetDatabase.GetLabels(textureImporter).Contains("Noesis"))
                {
                    Debug.LogWarning($"{uri} is missing Noesis label");
                }

                if (textureImporter.textureType == TextureImporterType.Sprite &&
                    textureImporter.spriteImportMode == SpriteImportMode.Single)
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(uri);
                    sprites.Add(sprite);
                }
                else
                {
                    Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(uri);
                    textures.Add(texture);
                }
            }
        }
    }

    static void AddAudio(string uri, ref List<AudioClip> audios)
    {
        if (!String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(uri)))
        {
            AudioClip audio = AssetDatabase.LoadAssetAtPath<AudioClip>(uri);

            if (audio != null)
            {
                audios.Add(audio);
            }
        }
    }

    static void AddVideo(string uri, ref List<VideoClip> videos)
    {
        if (!String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(uri)))
        {
            VideoClip video = AssetDatabase.LoadAssetAtPath<VideoClip>(uri);

            if (video != null)
            {
                videos.Add(video);
            }
        }
    }

    static void AddRive(string uri, ref List<NoesisRive> rives)
    {
        if (!String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(uri)))
        {
            NoesisRive rive = AssetDatabase.LoadAssetAtPath<NoesisRive>(uri);

            if (rive != null)
            {
                rives.Add(rive);
            }
        }
    }

    static void AddXaml(string uri, ref List<NoesisXaml> xamls)
    {
        if (!String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(uri)))
        {
            NoesisXaml xaml = AssetDatabase.LoadAssetAtPath<NoesisXaml>(uri);

            if (xaml != null)
            {
                xamls.Add(xaml);
            }
        }
    }

    static void AddShader(string uri, ref List<NoesisShader> shaders)
    {
        if (!String.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(uri)))
        {
            NoesisShader shader = AssetDatabase.LoadAssetAtPath<NoesisShader>(uri);

            if (shader != null)
            {
                shaders.Add(shader);
            }
        }
    }

    static void ScanDependencies(AssetImportContext ctx,
        out List<NoesisFont> fonts_, out List<Texture> textures_, out List<Sprite> sprites_,
        out List<AudioClip> audios_, out List<VideoClip> videos_, out List<NoesisRive> rives_,
        out List<NoesisXaml> xamls_, out List<NoesisShader> shaders_)
    {
        List<NoesisFont> fonts = new List<NoesisFont>();
        List<Texture> textures = new List<Texture>();
        List<Sprite> sprites = new List<Sprite>();
        List<AudioClip> audios = new List<AudioClip>();
        List<VideoClip> videos = new List<VideoClip>();
        List<NoesisRive> rives = new List<NoesisRive>();
        List<NoesisXaml> xamls = new List<NoesisXaml>();
        List<NoesisShader> shaders = new List<NoesisShader>();

        string filename = ctx.assetPath;

        try
        {
            if (HasExtension(filename, ".xaml"))
            {
                // Add dependency to code-behind, just the source, we don't need the artifact
                // Even if the file doesn't exist we add it to get a reimport first time code-behind is created
                ctx.DependsOnSourceAsset(filename + ".cs");
            }

            var dependencies = GetCachedDependencies(filename);

            foreach (var dep in dependencies.items)
            {
                if (dep.type == Dependency.Type.File)
                {
                    ctx.DependsOnArtifact(dep.uri);

                    AddXaml(dep.uri, ref xamls);
                    AddTexture(dep.uri, ref textures, ref sprites);
                    AddAudio(dep.uri, ref audios);
                    AddVideo(dep.uri, ref videos);
                    AddRive(dep.uri, ref rives);
                }
                else if (dep.type == Dependency.Type.Font)
                {
                    ctx.DependsOnArtifact(dep.uri);
                    AddFont(dep.uri, ref fonts);
                }
                else if (dep.type == Dependency.Type.UserControl)
                {
                    ctx.DependsOnArtifact(dep.uri);
                    AddXaml(dep.uri, ref xamls);
                }
                else if (dep.type == Dependency.Type.Shader)
                {
                    ctx.DependsOnArtifact(dep.uri);
                    AddShader(dep.uri, ref shaders);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        fonts_ = fonts;
        textures_ = textures;
        sprites_ = sprites;
        audios_ = audios;
        videos_ = videos;
        rives_ = rives;
        xamls_ = xamls;
        shaders_ = shaders;
    }

    public override void OnImportAsset(AssetImportContext ctx)
    {
        NoesisUnity.InitCore();

      #if DEBUG_IMPORTER
        Debug.Log($"=> Import {ctx.assetPath}");
      #endif

        NoesisXaml xaml = (NoesisXaml)ScriptableObject.CreateInstance<NoesisXaml>();
        xaml.uri = ctx.assetPath;
        xaml.content = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(ctx.assetPath));

        // Add dependencies
        List<NoesisFont> fonts;
        List<Texture> textures;
        List<Sprite> sprites;
        List<AudioClip> audios;
        List<VideoClip> videos;
        List<NoesisRive> rives;
        List<NoesisXaml> xamls;
        List<NoesisShader> shaders;

        ScanDependencies(ctx, out fonts, out textures, out sprites, out audios, out videos, out rives, out xamls, out shaders);

        xaml.xamls = xamls.Select(x => new NoesisXaml.Xaml { uri = AssetDatabase.GetAssetPath(x), xaml = x }).ToArray();
        xaml.textures = textures.Select(x => new NoesisXaml.Texture { uri = AssetDatabase.GetAssetPath(x), texture = x }).ToArray();
        xaml.sprites = sprites.Select(x => new NoesisXaml.Sprite { uri = AssetDatabase.GetAssetPath(x), sprite = x }).ToArray();
        xaml.audios = audios.Select(x => new NoesisXaml.Audio { uri = AssetDatabase.GetAssetPath(x), audio = x }).ToArray();
        xaml.videos = videos.Select(x => new NoesisXaml.Video { uri = AssetDatabase.GetAssetPath(x), video = x }).ToArray();
        xaml.rives = rives.Select(x => new NoesisXaml.Rive { uri = AssetDatabase.GetAssetPath(x), rive = x }).ToArray();
        xaml.fonts = fonts.Select(x => new NoesisXaml.Font { uri = AssetDatabase.GetAssetPath(x), font = x }).ToArray();
        xaml.shaders = shaders.Select(x => new NoesisXaml.Shader { uri = Path.GetFileName(AssetDatabase.GetAssetPath(x)), shader = x} ).ToArray();

        // Depends on global dictionary
        ctx.DependsOnCustomDependency("Noesis_ApplicationResources");

        ctx.AddObjectToAsset("XAML", xaml);
        ctx.SetMainObject(xaml);
    }

    static bool HasExtension(string filename, string extension)
    {
        return filename.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
    }

    static bool IsFont(string filename)
    {
        return HasExtension(filename, ".ttf") || HasExtension(filename, ".otf") || HasExtension(filename, ".ttc");
    }
}
