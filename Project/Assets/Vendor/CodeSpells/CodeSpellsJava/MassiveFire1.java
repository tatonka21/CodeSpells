import june.*;
import java.util.*;

public class MassiveFire1 extends Spell
{
  public void cast()
  { 
    Enchanted area   = getByName("Area 1");

    for(int i = 0; i < 10; i++)
      area.grow(1);

    EnchantedList list = area.findWithin();

    for(Enchanted thing : list)
    {
      thing.onFire(true); 
    }
  }
}
