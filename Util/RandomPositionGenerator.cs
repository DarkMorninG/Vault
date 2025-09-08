using Unity.Mathematics;

namespace Vault.Util {
    public static class RandomPositionGenerator {
        public static float2 RandomPositionInCircle(ref Random random, float radius, float2 position) {
            var randomPositionInCircle = RandomPositionInCircle(ref random, radius);
            return randomPositionInCircle + position;
        }

        public static float2 RandomPositionInCircle(ref Random random, float radius) {
            var theta = random.NextFloat(0f, 2f * math.PI);
            var r = radius * math.sqrt(random.NextFloat(0f, 1f));
            return new float2(r * math.cos(theta), r * math.sin(theta));
        }

        public static float3 RandomPositionInArea(ref Random random,
            float3 center,
            float innerRadius,
            float outerRadius) {
            var angle = random.NextFloat(0, math.PI * 2);
            var radius = math.sqrt(random.NextFloat(innerRadius * innerRadius, outerRadius * outerRadius));

            return new float3(math.cos(angle) * radius + center.x, math.sin(angle) * radius + center.y, 0);
        }

        public static float3 RandomPositionInArea(System.Random random,
            float3 center,
            float innerRadius,
            float outerRadius) {
            var angle = random.NextFloat(0, math.PI * 2);
            var radius = math.sqrt(random.NextFloat(innerRadius * innerRadius, outerRadius * outerRadius));

            return new float3(math.cos(angle) * radius + center.x, math.sin(angle) * radius + center.y, 0);
        }
    }
}