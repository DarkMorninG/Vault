using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace Vault {
    public static class VaultRawImage {
        public static void ChangeOpacity(this RawImage image, float opacity) {
            var newColor = new Color(image.color.r, image.color.g, image.color.b, opacity);
            image.color = newColor;
        }
    }
}