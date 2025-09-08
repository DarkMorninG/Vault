using UnityEngine;

namespace Vault {
    public static class VaultSpriteRenderer {
        public static void SetSpriteSize(this SpriteRenderer spriteRenderer, float pixelWidth, float pixelHeight) {
            var ppu = spriteRenderer.sprite.pixelsPerUnit;

            var worldWidth = pixelWidth / ppu;
            var worldHeight = pixelHeight / ppu;

            var spriteSize = spriteRenderer.sprite.bounds.size;
            var newScale = new Vector3(worldWidth / spriteSize.x, worldHeight / spriteSize.y, 1);
            spriteRenderer.transform.localScale = newScale;
        }
    }
}