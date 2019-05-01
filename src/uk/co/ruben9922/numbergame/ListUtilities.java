package uk.co.ruben9922.numbergame;

import org.apache.commons.lang3.StringUtils;
import org.jetbrains.annotations.NotNull;
import org.jetbrains.annotations.Nullable;
import uk.co.ruben9922.utilities.consoleutilities.InputUtilities;

import java.util.List;
import java.util.ListIterator;
import java.util.Scanner;

public class ListUtilities {
    public static <E> void printList(@NotNull List<E> list, boolean showIndices) {
        ListIterator<E> listIterator = list.listIterator();
        while (listIterator.hasNext()) {
            int index = listIterator.nextIndex();
            E element = listIterator.next();
            if (showIndices) {
                System.out.format("[%d] ", index);
            }
            System.out.format("%s\n", element.toString());
        }
    }

    public static <E> Integer chooseItem(Scanner scanner, @NotNull List<E> list, @Nullable String listName,
                                         @Nullable String itemName, @Nullable String prompt) {
        if (list.size() == 0) {
            return null;
        }

        // Assign default values to parameters if null
        if (listName == null) {
            listName = "list";
        }
        if (itemName == null) {
            itemName = "item";
        }
        if (prompt == null) {
            prompt = String.format("%s number [%d..%d]: ", StringUtils.capitalize(itemName), 0, list.size() - 1);
        }

        // If exactly one item in list then choose that item (don't bother asking user to choose item)
        if (list.size() == 1) {
            System.out.format("Only 1 %2$s in %1$s so picking that %2$s\n", listName, itemName);
            return 0;
        }

        // Simply print all items in list
        printList(list, true);

        return InputUtilities.inputInt(scanner, prompt, 0, list.size());
    }

    @Nullable
    public static <E> Integer chooseItem(Scanner scanner, @NotNull List<E> list, @Nullable String listName,
                                         @Nullable String itemName) {
        return chooseItem(scanner, list, listName, itemName, null);
    }
}
