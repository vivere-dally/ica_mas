package viveredally.agent;

import jade.core.AID;
import jade.core.Agent;
import jade.core.behaviours.CyclicBehaviour;
import jade.lang.acl.ACLMessage;
import jade.lang.acl.UnreadableException;
import jade.wrapper.StaleProxyException;
import lombok.SneakyThrows;
import viveredally.messages.TaskHandledMessage;
import viveredally.messages.TaskStartedMessage;
import viveredally.util.RoundRobinProvider;

import java.io.IOException;
import java.util.List;
import java.util.UUID;
import java.util.concurrent.CopyOnWriteArrayList;

public class MasterAgent extends Agent {
    private int numberOfWorkers;
    private List<AID> agents;
    private RoundRobinProvider roundRobinProvider;


    @SneakyThrows
    @Override
    protected void setup() {
        super.setup();
        this.handleSetup();

        final var self = this;
        this.addBehaviour(new CyclicBehaviour(this) {
            @Override
            public void action() {
                ACLMessage message = receive();
                if (message == null) {
                    block();
                } else {
                    try {
                        var obj = message.getContentObject();
                        if (obj instanceof TaskStartedMessage) {
                            var taskStartedMessage = (TaskStartedMessage) obj;
                            System.out.println(self.getAID() + ": task " + taskStartedMessage.getUuid() + " started by " + taskStartedMessage.getAid() + " at " + taskStartedMessage.getStartupTime());
                        } else if (obj instanceof TaskHandledMessage) {
                            var taskHandledMessage = (TaskHandledMessage) obj;
                            System.out.println(self.getAID() + ": task " + taskHandledMessage.getUuid() + " finished by " + taskHandledMessage.getAid() + " at " + taskHandledMessage.getStartupTime().plusSeconds(taskHandledMessage.getDurationInMillis() / 60));
                        } else if (obj instanceof UUID) {
                            var aid = agents.get(roundRobinProvider.getNext());

                            // Send task to worker
                            var requestMessage = new ACLMessage(ACLMessage.INFORM);
                            requestMessage.setContentObject(obj);
                            requestMessage.addReceiver(aid);
                            send(requestMessage);
                        }
                    } catch (UnreadableException | IOException e) {
                        e.printStackTrace();
                    }
                }
            }
        });
    }

    private void handleSetup() throws StaleProxyException {
        var args = (String[]) getArguments();
        this.numberOfWorkers = Integer.parseInt(args[0]);
        this.agents = new CopyOnWriteArrayList<>();
        for (int i = 1; i <= this.numberOfWorkers * 2; i += 2) {
            int ram = Integer.parseInt(args[i]);
            int cpu = Integer.parseInt(args[i + 1]);

            var workerAgent = this.getContainerController().createNewAgent(
                    String.format("Worker %d", (i / 2 + 1)),
                    WorkerAgent.class.getName(),
                    new Object[]{ram, cpu});

            this.agents.add(new AID(workerAgent.getName(), AID.ISLOCALNAME));
        }

        this.roundRobinProvider = new RoundRobinProvider(this.numberOfWorkers);
    }
}
