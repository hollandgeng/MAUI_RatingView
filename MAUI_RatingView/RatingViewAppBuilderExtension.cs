using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAUI_RatingView
{
    public static class RatingViewAppBuilderExtension
    { 
        public static MauiAppBuilder UseCustomRatingView(this MauiAppBuilder builder)
        {
            builder.ConfigureFonts(font => 
            {
                font.AddEmbeddedResourceFont(typeof(RatingViewAppBuilderExtension).Assembly , "MaterialIconsOutlined.otf" , "MaterialIconOutlined");
                font.AddEmbeddedResourceFont(typeof(RatingViewAppBuilderExtension).Assembly , "MaterialIconsSharp.otf" , "MaterialIconSharp");
                font.AddEmbeddedResourceFont(typeof(RatingViewAppBuilderExtension).Assembly , "MaterialIconsRound.otf" , "MaterialIconRound");
                font.AddEmbeddedResourceFont(typeof(RatingViewAppBuilderExtension).Assembly , "MaterialIconsTwoTone.otf" , "MaterialIconTwoTone");
                font.AddEmbeddedResourceFont(typeof(RatingViewAppBuilderExtension).Assembly , "MaterialIconsRegular.ttf" , "MaterialIconRegular");
            });

            return builder;
        }  
    }
}
