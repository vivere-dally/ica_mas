package viveredally.domain;

import jade.core.AID;
import lombok.Data;
import lombok.RequiredArgsConstructor;

import java.io.Serializable;
import java.time.LocalDateTime;
import java.util.UUID;

@Data
@RequiredArgsConstructor
public class Task implements Serializable {
    private final UUID uuid;
    private final AID aid;
    private final Specs specs;
    private LocalDateTime queuedTime;
    private LocalDateTime startedTime;
    private LocalDateTime finishedTime;
    private TaskState state;
}
