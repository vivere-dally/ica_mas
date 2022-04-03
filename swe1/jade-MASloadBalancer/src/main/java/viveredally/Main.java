package viveredally;

import jade.core.*;
import jade.core.Runtime;
import jade.wrapper.AgentController;
import jade.wrapper.ContainerController;
import jade.wrapper.StaleProxyException;

import viveredally.agent.MasterAgent;

public class Main {
    public static void main(String[] args) {
        Runtime runtime = Runtime.instance();
        Profile profile = new ProfileImpl();
        profile.setParameter(Profile.MAIN_HOST, "localhost");
        profile.setParameter(Profile.MAIN_PORT, "3000");
        profile.setParameter(Profile.GUI, "true");

        ContainerController containerController = runtime.createMainContainer(profile);
        try {
            AgentController agentController = containerController.createNewAgent("Master", MasterAgent.class.getName(), generateArgs());
            agentController.start();
        } catch (StaleProxyException e) {
            e.printStackTrace();
        }
    }

    private static Object[] generateArgs() {
        int n = 3;
        var args = new Object[n * 2 + 1];

        args[0] = n;
        for (int i = 1; i <= n * 2; i += 2) {
            args[i] = i * 100;
            args[i + 1] = i * 10;

        }

        return args;
    }
}
