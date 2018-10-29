using Plugin.ShareFile.Abstractions;
using System;

namespace Plugin.ShareFile
{
  /// <summary>
  /// Cross platform ShareFile implemenations
  /// </summary>
  public class CrossShareFile
  {
    static readonly Lazy<IShareFile> Implementation = new Lazy<IShareFile>(CreateShareFile, System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IShareFile Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IShareFile CreateShareFile()
    {
#if PORTABLE
        return null;
#else
        return new ShareFileImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
