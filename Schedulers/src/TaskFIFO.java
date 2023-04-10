import java.util.ArrayList;
import java.util.LinkedList;

public class TaskFIFO implements Runnable {
    int[] sequence;
    int maxMemoryFrames;
    int maxPageReference;
    int[] pageFaults;
    int numberOfFaults = 0;
    LinkedList currentPages = new LinkedList();

    public TaskFIFO(int[] sequence , int maxMemoryFrames, int maxPageReference, int[] pageFaults) {
        this.sequence = sequence;
        this.maxMemoryFrames = maxMemoryFrames;
        this.maxPageReference = maxPageReference;
        this.pageFaults = pageFaults;
    }

    @Override
    public void run() {
        for (int i = 0; i < sequence.length; i++) {
            if (!currentPages.contains(sequence[i])) {
                currentPages.add(sequence[i]);
                numberOfFaults++;
                    if (currentPages.size() > maxMemoryFrames) {
                    currentPages.removeFirst();
                    }
                }
            }
        pageFaults[maxMemoryFrames - 1] = numberOfFaults;
        // System.out.println(f + " memory frames and " + pageFaults[f - 1] + " page faults");
        }
    }


