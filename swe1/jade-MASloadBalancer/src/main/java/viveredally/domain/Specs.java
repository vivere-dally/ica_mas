package viveredally.domain;

import lombok.*;

import java.io.Serializable;

@Data
@AllArgsConstructor
@NoArgsConstructor
@With
@Builder(toBuilder=true)
public class Specs implements Serializable {
    private int ram;
    private int cpu;

    public void add(Specs o) {
        this.ram += o.getRam();
        this.cpu += o.getCpu();
    }

    public void sub(Specs o) {
        this.ram -= o.getRam();
        this.cpu -= o.getCpu();
    }

    public boolean isLower(Specs o) {
        return this.ram <= o.ram && this.cpu <= o.cpu;
    }
}
