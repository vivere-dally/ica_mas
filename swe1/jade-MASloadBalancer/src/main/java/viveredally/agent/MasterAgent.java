package viveredally.agent;

import jade.core.AID;
import jade.core.Agent;
import jade.core.behaviours.CyclicBehaviour;
import jade.lang.acl.ACLMessage;
import jade.wrapper.StaleProxyException;
import lombok.SneakyThrows;
import viveredally.messages.TerminateMessage;
import viveredally.util.RoundRobinProvider;

import java.util.Arrays;
import java.util.List;
import java.util.Random;
import java.util.UUID;
import java.util.concurrent.CopyOnWriteArrayList;

import static viveredally.util.Logger.log;

public class MasterAgent extends Agent {
    private List<AID> agents;
    private RoundRobinProvider roundRobinProvider;

    @SneakyThrows
    @Override
    protected void setup() {
        super.setup();
        this.handleSetup();

        this.addBehaviour(new CyclicBehaviour(this) {
            @Override
            public void action() {
                ACLMessage message = receive();
                if (message == null) {
                    block();
                } else {
                    var content = message.getContent();
                    var aid = agents.get(roundRobinProvider.getNext());

                    // Send task to worker
                    var requestMessage = new ACLMessage(ACLMessage.INFORM);
                    requestMessage.setContent(content);
                    requestMessage.addReceiver(aid);
                    send(requestMessage);
                }
            }
        });

        generateLoad();
    }

    @SneakyThrows
    @Override
    protected void takeDown() {
        super.takeDown();

        for (AID agent : this.agents) {
            var requestMessage = new ACLMessage(ACLMessage.INFORM);
            requestMessage.setContentObject(new TerminateMessage());
            requestMessage.addReceiver(agent);
            send(requestMessage);
        }

        log(getAID(), "terminated");
    }

    private void handleSetup() throws StaleProxyException {
        var args = Arrays.stream(getArguments()).toArray(Integer[]::new);
        int numberOfWorkers = args[0];
        this.agents = new CopyOnWriteArrayList<>();
        for (int i = 1; i <= numberOfWorkers * 2; i += 2) {
            int ram = args[i];
            int cpu = args[i + 1];

            var workerAgentName = String.format("Worker %d", (i / 2 + 1));
            var workerAgent = this.getContainerController().createNewAgent(
                    workerAgentName,
                    WorkerAgent.class.getName(),
                    new Object[]{ram, cpu});
            workerAgent.start();

            this.agents.add(new AID(workerAgentName, AID.ISLOCALNAME));
        }

        this.roundRobinProvider = new RoundRobinProvider(numberOfWorkers);
    }

    private void generateLoad() {
        new Thread(() -> {
            var masterAID = new AID("Master", AID.ISLOCALNAME);
            for (int i = 0; i < 50; i++) {
                var requestMessage = new ACLMessage(ACLMessage.INFORM);
                requestMessage.setContent(UUID.randomUUID().toString());
                requestMessage.addReceiver(masterAID);
                send(requestMessage);
                try {
                    Thread.sleep(new Random().nextInt(500));
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }

//            doDelete();
        }).start();
    }
}
