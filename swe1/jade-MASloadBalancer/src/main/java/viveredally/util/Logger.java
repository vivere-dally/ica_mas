package viveredally.util;

import jade.core.AID;
import viveredally.domain.Task;

import java.time.LocalDateTime;

public class Logger {
    public static void log(AID aid, String message) {
        System.out.println(aid + ": " + message);
    }

    public static void log(Task task) {
        String s = task.getAid() +
                ": Task " +
                task.getUuid() +
                " " +
                task.getState().name() +
                " at " +
                getDate(task);

        System.out.println(s);
    }

    private static LocalDateTime getDate(Task task) {
        return switch (task.getState()) {
            case STARTED -> task.getStartedTime();
            case QUEUED -> task.getQueuedTime();
            case FINISHED -> task.getFinishedTime();
        };
    }
}
