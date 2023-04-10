import java.util.ArrayList;
import java.util.LinkedList;

public class TaskMRU implements Runnable {
    int[] sequence;
    int maxMemoryFrames;
    int maxPageReference;
    int[] pageFaults;
    int numberOfFaults = 0;
    ArrayList currentPages = new ArrayList();

    public TaskMRU(int[] sequence , int maxMemoryFrames, int maxPageReference, int[] pageFaults) {
        this.sequence = sequence;
        this.maxMemoryFrames = maxMemoryFrames;
        this.maxPageReference = maxPageReference;
        this.pageFaults = pageFaults;
    }

    @Override
    public void run() {
        int mru = sequence[0];
        for (int i = 0; i < sequence.length; i++) {
            if(i > 1) {
            mru = sequence[i - 1];}
            if (!currentPages.contains(sequence[i])) {
                currentPages.add(sequence[i]);
                numberOfFaults++;
                if (currentPages.size() > maxMemoryFrames) {
                    for(int j = 0 ; j < currentPages.size(); j++) {
                        if(currentPages.get(j).equals(mru)) {
                            currentPages.remove(j);
                        }
                    }
                }
            }
        }
        pageFaults[maxMemoryFrames - 1] = numberOfFaults;
        // System.out.println(f + " memory frames and " + pageFaults[f - 1] + " page faults");
    }
}
