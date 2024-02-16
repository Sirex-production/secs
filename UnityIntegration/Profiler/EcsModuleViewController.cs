#if SECS_ENABLE_PROFILING
using System;
using Unity.Profiling;
using Unity.Profiling.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Secs.Profiler
{
    public sealed class EcsModuleViewController : ProfilerModuleViewController
    {
        private readonly ProfilerCounterDescriptor _gcAllocatedMemoryInFrameCounterDescriptor = new ("GC Allocated In Frame", ProfilerCategory.Memory);
        private readonly ProfilerCounterDescriptor _scriptCounterDescriptor = new ("Scripts", ProfilerCategory.Scripts);
        
        private long _gcReservedBytes;
        private long _gcUsedBytes;
        
        private Label _selectedFrameIndexLabel;
        private Label _gcReservedBytesLabel;
        private Label _gcUsedBytesLabel;
        
        public EcsModuleViewController(ProfilerWindow profilerWindow) : base(profilerWindow)
        {
            ProfilerWindow.SelectedFrameIndexChanged -= SelectedFrameIndexChanged;
            ProfilerWindow.SelectedFrameIndexChanged += SelectedFrameIndexChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ProfilerWindow.SelectedFrameIndexChanged -= SelectedFrameIndexChanged;
            }
            
            base.Dispose(disposing);
        }

        private void SelectedFrameIndexChanged(long selectedFrameIndex)
        {
            int selectedFrameIndexInt32 = Convert.ToInt32(selectedFrameIndex);
            using var frameData = UnityEditorInternal.ProfilerDriver.GetRawFrameDataView(selectedFrameIndexInt32, 0);
            
            if (frameData is not { valid: true })
                return;

            int gcReservedBytesMarkerId = frameData.GetMarkerId(_gcAllocatedMemoryInFrameCounterDescriptor.Name);
            _gcReservedBytes = frameData.GetCounterValueAsLong(gcReservedBytesMarkerId);

            int gcUsedBytesMarkerId = frameData.GetMarkerId(_scriptCounterDescriptor.Name);
            _gcUsedBytes = frameData.GetCounterValueAsLong(gcUsedBytesMarkerId);

            UpdateView(selectedFrameIndex);
        }
        
        protected override VisualElement CreateView()
        {
            var rootView = new VisualElement();
      
            _selectedFrameIndexLabel = new Label($"Selected Frame Index: {-1}");
            _gcReservedBytesLabel = new Label($"GC Reserved Bytes: {0}");
            _gcUsedBytesLabel = new Label($"GC Used Bytes: {0}");
           
            rootView.Add(_selectedFrameIndexLabel);
            rootView.Add(_gcReservedBytesLabel);
            rootView.Add(_gcUsedBytesLabel);
            
            return rootView;
        }
        
        private void UpdateView(long selectedFrameIndex)
        {
            _selectedFrameIndexLabel.text = $"Selected Frame Index: {selectedFrameIndex}";
            _gcReservedBytesLabel.text = $"GC Reserved Bytes: {_gcReservedBytes}";
            _gcUsedBytesLabel.text = $"GC Used Bytes: {_gcUsedBytes}";
        }
    }
}        
#endif