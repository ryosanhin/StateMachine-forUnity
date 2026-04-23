namespace Ryosanhin.TimeControllers.Wrappers{
	public sealed class GlobalTime{
		public TimeController Entity{get;}
		public GlobalTime(TimeController entity){
			this.Entity=entity;
		}
	}
	
	public sealed class InGameTime{
		public TimeController Entity{get;}
		public InGameTime(TimeController entity){
			this.Entity=entity;
		}
	}
	
	public sealed class OutGameTime{
		public TimeController Entity{get;}
		public OutGameTime(TimeController entity){
			this.Entity=entity;
		}
	}
}