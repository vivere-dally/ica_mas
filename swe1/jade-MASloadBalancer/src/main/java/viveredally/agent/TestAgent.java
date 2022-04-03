package viveredally.agent;

import jade.core.Agent;

public class TestAgent extends Agent {

    @Override
    protected void setup() {
        super.setup();

        System.out.println("setup " + getAID());
    }

    @Override
    protected void takeDown() {
        super.takeDown();

        System.out.println("takeDown " + getAID());
    }
}
