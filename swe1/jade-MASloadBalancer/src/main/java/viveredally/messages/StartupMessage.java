package viveredally.messages;


import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import viveredally.domain.Specs;

import java.util.List;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class StartupMessage implements Message {
    private int numberOfWorkers;
    private List<Specs> workerSpecs;
}
