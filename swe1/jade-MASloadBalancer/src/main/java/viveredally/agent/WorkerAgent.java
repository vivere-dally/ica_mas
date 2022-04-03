package viveredally.agent;

import jade.core.Agent;
import jade.core.behaviours.CyclicBehaviour;
import jade.lang.acl.ACLMessage;
import lombok.SneakyThrows;
import viveredally.domain.Specs;
import viveredally.domain.Task;
import viveredally.domain.TaskState;
import viveredally.util.RandomSpecsProvider;

import java.time.LocalDateTime;
import java.util.Random;
import java.util.UUID;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.LinkedBlockingDeque;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.concurrent.locks.ReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock;

public class WorkerAgent extends Agent {
    private RandomSpecsProvider randomSpecsProvider;
    private Specs workerSpecs;
    private Specs specs;
    private ExecutorService executorService;
    private LinkedBlockingDeque<Task> tasks;
    private ReadWriteLock readWriteLock;
    private AtomicBoolean isAlive;

    @Override
    protected void setup() {
        super.setup();

        int ram = Integer.parseInt((String) getArguments()[0]);
        int cpu = Integer.parseInt((String) getArguments()[1]);
        this.workerSpecs = new Specs(ram, cpu);
        this.specs = new Specs(0, 0);
        this.randomSpecsProvider = new RandomSpecsProvider(workerSpecs);


        this.executorService = Executors.newFixedThreadPool(2);
        this.tasks = new LinkedBlockingDeque<>();
        this.readWriteLock = new ReentrantReadWriteLock(true);
        this.isAlive = new AtomicBoolean();

        this.isAlive.set(true);
        final var self = this;
        this.addBehaviour(new CyclicBehaviour(this) {
            @SneakyThrows
            @Override
            public void action() {
                ACLMessage message = receive();
                if (message == null) {
                    block();
                } else {
                    var obj = message.getContentObject();
                    if (obj instanceof UUID) {
                        var uuid = (UUID) obj;
                        Specs taskSpecs = self.randomSpecsProvider.getTaskSpecs();

                        Task task = new Task(uuid, self.getAID(), taskSpecs);
                        task.setState(TaskState.QUEUED);
                        tasks.addLast(task);
                        synchronized (self) {
                            self.notify();
                        }
                    }
                }
            }
        });

        this.compute();
    }

    private void compute() {
        Thread thread = new Thread(() -> {
            while (isAlive.get()) {
                if (tasks.isEmpty()) {
                    synchronized (this) {
                        try {
                            this.wait();
                        } catch (InterruptedException e) {
                            e.printStackTrace();
                        }
                    }

                    continue;
                }

                Specs specsClone;
                try {
                    this.readWriteLock.readLock().lock();
                    specsClone = this.specs.toBuilder().build();
                } finally {
                    this.readWriteLock.readLock().unlock();
                }

                specsClone.add(this.tasks.peekFirst().getSpecs());
                if (!specsClone.isLower(this.workerSpecs)) {
                    // Cannot handle currently
                    continue;
                }

                // Read-write lock
                final var task = this.tasks.removeFirst();
                try {
                    this.readWriteLock.writeLock().lock();
                    this.specs.add(task.getSpecs());
                } finally {
                    this.readWriteLock.writeLock().unlock();
                }

                // Compute
                this.executorService.submit(() -> {
                    task.setStartedTime(LocalDateTime.now());
                    task.setState(TaskState.STARTED);
                    try {
                        // Simulate workload
                        Thread.sleep(new Random().nextInt(5000));
                    } catch (InterruptedException e) {
                        e.printStackTrace();
                    }

                    task.setFinishedTime(LocalDateTime.now());
                    task.setState(TaskState.FINISHED);
                    try {
                        this.readWriteLock.writeLock().lock();
                        this.specs.sub(task.getSpecs());
                    } finally {
                        this.readWriteLock.writeLock().unlock();
                    }
                });
            }
        });

        thread.start();
    }
}
