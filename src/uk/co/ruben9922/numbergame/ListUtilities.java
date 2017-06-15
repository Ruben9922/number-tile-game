package uk.co.ruben9922.numbergame;

import java.util.List;
import java.util.ListIterator;

public class ListUtilities {
    public static <E> void printList(List<E> list, boolean showIndices) {
        ListIterator<E> listIterator = list.listIterator();
        while (listIterator.hasNext()) {
            E element = listIterator.next();
            int index = listIterator.nextIndex();
            if (showIndices) {
                System.out.format("[%d] ", index);
            }
            System.out.format("%s\n", element.toString());
        }
    }
}
