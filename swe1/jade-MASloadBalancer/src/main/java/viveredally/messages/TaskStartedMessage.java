package viveredally.messages;

import jade.core.AID;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import viveredally.domain.Specs;

import java.time.LocalDateTime;
import java.util.UUID;


@AllArgsConstructor
@NoArgsConstructor
@Data
public class TaskStartedMessage implements Message {
    private UUID uuid;
    private LocalDateTime startupTime;
    private AID aid;
    private Specs specs;
}
