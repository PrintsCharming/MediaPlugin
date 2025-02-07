﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace MPowerKit.MediaPlugin;

/// <summary>
/// Cross platform Media implemenations
/// </summary>
public static class Media
{
    private static Lazy<IMedia> _implementation = new(CreateMedia, LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Gets if the plugin is supported on the current platform.
    /// </summary>
    public static bool IsSupported => _implementation.Value is not null;

    /// <summary>
    /// Current plugin implementation to use
    /// </summary>
    public static IMedia Current => _implementation.Value;

    static IMedia CreateMedia()
    {
#if ANDROID || MACIOS || WINDOWS
        return new MediaImplementation();
#else
        throw new NotImplementedException("MediaPlugin is not implemented for current platform");
#endif
    }

    public static MauiAppBuilder UseMpowerKitMediaPlugin(this MauiAppBuilder builder)
    {
        return UseMpowerKitMediaPlugin(builder, false);
    }

    public static MauiAppBuilder UseMpowerKitMediaPlugin(this MauiAppBuilder builder, bool registerInterface)
    {
        if (registerInterface)
        {
            builder.Services.AddSingleton(() => Current);
        }

#if ANDROID
        builder.ConfigureLifecycleEvents(lifecycle =>
        {
            lifecycle.AddAndroid(android =>
            {
                android.OnActivityResult((activity, requestCode, resultCode, data) =>
                {
                    MediaImplementation.SendActivityResult(requestCode, resultCode, data);
                });
            });
        });
#endif

        return builder;
    }
}