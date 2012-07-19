import june.*;

public class MySpell1 extends Spell{
  public void cast(){
    Enchanted e = getTarget();
    
    while(true){
      moveEast(e);
      moveWest(e);
    }
  }
  
  public void moveEast(Enchanted e){
    for(int i = 0; i <100; i++)
    {
      e.move(Direction.east(),.2f);
    } 
  } 
  
  public void moveWest(Enchanted e){
    for(int i = 0; i <100; i++)
    {
      e.move(Direction.west(),.2f);
    } 
  } 
}

