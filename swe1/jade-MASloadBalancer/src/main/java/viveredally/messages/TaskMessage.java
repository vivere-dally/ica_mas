package viveredally.messages;


import jade.core.AID;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.experimental.SuperBuilder;
import viveredally.domain.Specs;

import java.time.LocalDateTime;
import java.util.UUID;

@Data
@SuperBuilder
@NoArgsConstructor
public abstract class TaskMessage implements Message {
    private UUID uuid;
    private LocalDateTime startupTime;
    private AID aid;
    private Specs specs;
}
