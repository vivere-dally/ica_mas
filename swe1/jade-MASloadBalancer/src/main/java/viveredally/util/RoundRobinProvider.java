package viveredally.util;

public class RoundRobinProvider {
    private final int n;
    private int current;

    public RoundRobinProvider(int n) {
        this.n = n;
        this.current = 0;
    }

    public int getNext() {
        int currentCopy = current;
        current = (current + 1) % n;
        return currentCopy;
    }
}
