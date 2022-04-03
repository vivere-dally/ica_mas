package viveredally.util;

import viveredally.domain.Specs;

import java.util.Random;

public class RandomSpecsProvider {
    private final Specs workerSpecs;
    private final Random random;

    public RandomSpecsProvider(Specs workerSpecs) {
        this.workerSpecs = workerSpecs;
        this.random = new Random();
    }

    public Specs getTaskSpecs() {
        int ram = random.nextInt(workerSpecs.getRam());
        int cpu = random.nextInt(workerSpecs.getCpu());
        return new Specs(ram, cpu);
    }
}
